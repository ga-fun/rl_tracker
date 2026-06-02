using System.Net.Sockets;
using GuillaumeAst.Utils;

namespace GuillaumeAst.Network;

public sealed class Connection : Notifier
{
	public enum ExceptionAction
	{
		Continue,
		Stop
	}

	public enum ConnectionStatus
	{
		Connecting,		// Initial connection try
		Connected,		// Connected
		Reconnecting,	// Trying to reconnect after initial connection success
		Disconnecting,	// Voluntary disconnecting
		Disconnected,	// Not connected and not trying to connect
		Count
	}

	private sealed class State(int port, CancellationToken token)
	{
		internal readonly int Port = port;
		internal readonly CancellationToken Token = token;
		internal Client? Client { get; set; }
		internal bool ShouldWait { get; set; } = false;
	}

	private const int ConnectionRetryDelay = 1000;

	public event Action<Exception>? Reconnecting = null;
	public event Action<string>? MessageReceived = null;
	public ConnectionStatus Status
	{
		get;
		private set
		{
			if (field != value)
			{
				field = value;
				NotifyChange();
			}
		}
	} = ConnectionStatus.Disconnected;

	private readonly SemaphoreSlim _publicGate = new(1, 1);
	private readonly SemaphoreSlim _cleanupGate = new(1, 1);
	private CancellationTokenSource? _tokenSource = null;
	private Task? _listeningTask = null;
	private Task? _cleanupTask = null;
	private Func<Exception, ExceptionAction>? _onException = null;
	private string? _lastExceptionMessage;

	public async Task StartAsync(int port, Func<Exception, ExceptionAction> onException)
	{
		ArgumentNullException.ThrowIfNull(onException);

		await _publicGate.WaitAsync();
		try
		{
			if (_listeningTask != null)
			{
				await StopInternalAsync(false);
			}
			Status = ConnectionStatus.Connecting;
			_tokenSource = new();
			_onException = onException;
			State state = new(port, _tokenSource.Token);
			_listeningTask = TryConnectionLoopAsync(state);
		}
		finally
		{
			_publicGate.Release();
		}
	}

	public async Task StopAsync()
	{
		await _publicGate.WaitAsync();
		try
		{
			await StopInternalAsync(false);
		}
		finally
		{
			_publicGate.Release();
		}
	}

	private async Task TryConnectionLoopAsync(State state)
	{
		while (!state.Token.IsCancellationRequested)
		{
			try
			{
				await ConnectionLoopAsync(state);
				return;
			}
			catch (Exception exception)
			{
				if (_onException!(exception) != ExceptionAction.Continue)
				{
					await StopInternalAsync(true);
					return;
				}
			}
		}
	}

	private async Task ConnectionLoopAsync(State state)
	{
		while (!state.Token.IsCancellationRequested)
		{
			state.ShouldWait = false;
			try
			{
				await ConnectAsync(state);
				await ListenAsync(state);
			}
			catch (OperationCanceledException)
			{
				break;
			}
			catch (Exception exception) when (exception
				is IOException
				or SocketException)
			{
				ConnectionFailed(state, exception);
			}
			finally
			{
				await DisconnectAsync(state);
			}
			if (state.ShouldWait && !state.Token.IsCancellationRequested)
			{
				await TryWaitAsync(state);
			}
		}
	}

	private async Task ConnectAsync(State state)
	{
		state.Client = new(state.Port);
		await state.Client.ConnectAsync(state.Token);
		Status = ConnectionStatus.Connected;
		_lastExceptionMessage = null;
	}
	
	private async Task ListenAsync(State state)
	{
		Client client = state.Client
			?? throw new InvalidOperationException("Connection client is null");
		
		while (!state.Token.IsCancellationRequested)
		{
			string message = await client.ReceiveAsync(state.Token);
			TryEvent(MessageReceived, message);
		}
	}

	private void ConnectionFailed(State state, Exception exception)
	{
		if (Status == ConnectionStatus.Connected)
		{
			Status = ConnectionStatus.Reconnecting;
			Log.PrintRed($"Connection lost: {exception.GetType().Name}: {exception.Message}");
			TryEvent(Reconnecting, exception);
		}
		else if (_lastExceptionMessage == null || _lastExceptionMessage != exception.Message)
		{
			_lastExceptionMessage = exception.Message;
			Log.PrintRed($"Connection failed: {exception.GetType().Name}: {exception.Message}");
			Log.PrintYellow($"Retrying every {ConnectionRetryDelay} ms...");
		}
		state.ShouldWait = true;
	}

	private static async Task DisconnectAsync(State state)
	{
		Client? client = state.Client;

		if (client != null)
		{
			try
			{
				client.Close();
			}
			catch (Exception exception) when (exception
				is IOException
				or SocketException)
			{
				// Already disconnected
			}
			catch (ObjectDisposedException)
			{
				//  Already disposed
			}
			finally
			{
				client.Dispose();
				state.Client = null;
			}
		}
	}

	private static async Task TryWaitAsync(State state)
	{
		try
		{
			await Task.Delay(ConnectionRetryDelay, state.Token);
		}
		catch (OperationCanceledException)
		{
			// Operation has been cancelled by caller
		}
	}

	private void TryEvent<T>(Action<T>? callback, T arg)
	{
		try
		{
			callback?.Invoke(arg);
		}
		catch (Exception exception)
		{
			if (_onException!(exception) == ExceptionAction.Stop)
			{
				throw;
			}
		}
	}

	// calledFromListeningTask == true means this method must not await _listeningTask or cleanupTask to avoid deadlock
	private async Task StopInternalAsync(bool calledFromListeningTask)
	{
		Task? cleanupTask = null;

		await _cleanupGate.WaitAsync();
		try
		{
			if (Status == ConnectionStatus.Disconnected)
			{
				return;
			}
			if (Status == ConnectionStatus.Disconnecting)
			{
				cleanupTask = _cleanupTask;
			}
			else
			{
				Status = ConnectionStatus.Disconnecting;
				cleanupTask = UnsafeCleanupAsync(calledFromListeningTask);
				_cleanupTask = cleanupTask;
			}
		}
		finally
		{
			_cleanupGate.Release();
		}
		if (!calledFromListeningTask && cleanupTask != null)
		{
			await cleanupTask;
		}
	}

	private async Task UnsafeCleanupAsync(bool calledFromListeningTask)
	{
		try
		{
			_tokenSource?.Cancel();
		}
		catch (ObjectDisposedException)
		{
			// Token source is already disposed
		}
		finally
		{
			if (!calledFromListeningTask && _listeningTask != null)
			{
				await _listeningTask;
			}
			try
			{
				_tokenSource?.Dispose();
			}
			finally
			{
				_tokenSource = null;
				_listeningTask = null;
				_cleanupTask = null;
				Status = ConnectionStatus.Disconnected;
			}
		}
	}
}
