using System.ComponentModel;
using System.Net;
using GuillaumeAst.Utils;

namespace GuillaumeAst.Network;

public sealed class Connection : Notifier
{
	public enum ClientType
	{
		TCP
	}

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

	private const int ConnectionRetryDelay = 1000;
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
	public event Action<Exception>? Reconnecting = null;
	public event Action<byte[]>? BytesReceived = null;
	private readonly Func<Exception, ExceptionAction> _onException;
	private readonly SemaphoreSlim _publicGate = new(1, 1);
	private readonly SemaphoreSlim _cleanupGate = new(1, 1);
	private readonly IPAddress _ipAddress;
	private readonly int _port;
	private readonly ClientType _clientType;
	private ITransportClient? _client = null;
	private CancellationTokenSource? _tokenSource = null;
	private Task? _listeningTask = null;
	private Task? _cleanupTask = null;
	private string? _lastExceptionMessage;
	private bool _shouldWait = false;

	public Connection(ClientType clientType, IPAddress ipAddress, int port, Func<Exception, ExceptionAction> onException)
	{
		_clientType = clientType;
		_ipAddress = ipAddress;
		_port = port;
		_onException = onException;
	}

	public async Task StartAsync()
	{
		await _publicGate.WaitAsync();
		try
		{
			if (_listeningTask != null)
			{
				await StopInternalAsync(false);
			}
			Status = ConnectionStatus.Connecting;
			_tokenSource = new();
			_listeningTask = TryConnectionLoopAsync(_tokenSource.Token);
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

	private async Task TryConnectionLoopAsync(CancellationToken token)
	{
		while (!token.IsCancellationRequested)
		{
			try
			{
				await ConnectionLoopAsync(token);
				return;
			}
			catch (Exception exception)
			{
				if (_onException(exception) != ExceptionAction.Continue)
				{
					await StopInternalAsync(true);
					return;
				}
			}
		}
	}

	private async Task ConnectionLoopAsync(CancellationToken token)
	{
		while (!token.IsCancellationRequested)
		{
			_shouldWait = false;
			try
			{
				await ConnectAsync(token);
				await ListenAsync(token);
			}
			catch (OperationCanceledException)
			{
				break;
			}
			catch (Exception exception) when (exception
				is IOException
				or System.Net.Sockets.SocketException)
			{
				ConnectionFailed(exception);
			}
			finally
			{
				await DisconnectAsync(CancellationToken.None);
			}
			if (_shouldWait && !token.IsCancellationRequested)
			{
				await TryWaitAsync(token);
			}
		}
	}

	private async Task ConnectAsync(CancellationToken token)
	{
		if (_clientType == ClientType.TCP)
		{
			_client = new TcpTransportClient(_ipAddress, _port);
		}
		else
		{
			throw new InvalidEnumArgumentException("Only TCP clients are handled for now");
		}
		await _client.ConnectAsync(token);
		Status = ConnectionStatus.Connected;
		_lastExceptionMessage = null;
	}
	
	private async Task ListenAsync(CancellationToken token)
	{
		if (_client == null)
		{
			throw new InvalidOperationException("_client is not set");
		}
		
		while (!token.IsCancellationRequested)
		{
			byte[] bytes = await _client.ReceiveAsync(token);
			TryEvent(BytesReceived, bytes);
		}
	}

	private void ConnectionFailed(Exception exception)
	{
		if (Status == ConnectionStatus.Connected)
		{
			Status = ConnectionStatus.Reconnecting;
			Log.Write(Log.Level.Warning, $"Connection lost: {exception.GetType().Name}: {exception.Message}");
			TryEvent(Reconnecting, exception);
		}
		else if (_lastExceptionMessage == null || _lastExceptionMessage != exception.Message)
		{
			_lastExceptionMessage = exception.Message;
			Log.Write(Log.Level.Warning, $"Connection failed: {exception.GetType().Name}: {exception.Message}");
			Log.Write(Log.Level.Debug, $"Retrying every {ConnectionRetryDelay} ms...");
		}
		_shouldWait = true;
	}

	private async Task DisconnectAsync(CancellationToken token)
	{
		if (_client != null)
		{
			await _client.CloseAsync(token);
			_client = null;
		}
	}

	private static async Task TryWaitAsync(CancellationToken token)
	{
		try
		{
			await Task.Delay(ConnectionRetryDelay, token);
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
			if (_onException(exception) == ExceptionAction.Stop)
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
			if (_client != null)
			{
				await _client.CloseAsync(CancellationToken.None);
				_client = null;
			}
		}
		catch (ObjectDisposedException)
		{
			// Token source or client is already disposed
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
				_listeningTask = null;
				_cleanupTask = null;
				Status = ConnectionStatus.Disconnected;
			}
		}
	}
}
