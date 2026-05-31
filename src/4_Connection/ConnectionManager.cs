namespace RlTracker.Core;

internal sealed class ConnectionManager
{
	private sealed class Connection(int port, CancellationToken token)
	{
		internal int Port { get; } = port;
		internal CancellationToken Token { get; } = token;
		internal Client? Client { get; set; }
		internal bool Connected { get; set; } = false;
	}

	private const int ConnectionRetryDelay = 1000;
	private CancellationTokenSource? _cancellationTokenSource = null;
	private Task? _listeningTask = null;
	private Action? _onConnect = null;
	private Action<string>? _onMessage = null;
	private Action? _onDisconnect = null;

	internal void StartAsync(
		int port,
		Action? onConnect,
		Action<string>? onMessage,
		Action? onDisconnect)
	{
		if (_listeningTask != null)
			return;
		
		Log.Print("Connecting...");
		_onConnect = onConnect;
		_onMessage = onMessage;
		_onDisconnect = onDisconnect;
		_cancellationTokenSource = new();
		_listeningTask = ConnectionLoopAsync(port, _cancellationTokenSource.Token);
	}

	internal async Task StopAsync()
	{
		Log.Print("Disconnecting...");
		_cancellationTokenSource?.Cancel();
		if (_listeningTask != null)
			await _listeningTask;

		_cancellationTokenSource?.Dispose();
		_cancellationTokenSource = null;
		_listeningTask = null;
		Log.PrintGreen("Disconnected.");
	}

	private async Task ConnectionLoopAsync(int port, CancellationToken token)
	{
		Connection connection = new(port, token);

		while (!token.IsCancellationRequested)
		{
			try
			{
				await ConnectAsync(connection);
				await ListenAsync(connection);
			}
			catch (OperationCanceledException)
			{
				break;
			}
			catch (Exception exception)
			{
				Log.PrintYellow($"Connection failed: {exception.GetType().Name}: {exception.Message}.");
				if (!await RetryAsync(connection))
					break;
			}
			finally
			{
				await DisconnectAsync(connection);
			}
		}
	}

	private async Task ConnectAsync(Connection connection)
	{
		connection.Client = new(connection.Port);
		await connection.Client.ConnectAsync(connection.Token);
		connection.Connected = true;
		_onConnect?.Invoke();
		Log.PrintGreen("Connected.");
	}
	
	private async Task ListenAsync(Connection connection)
	{
		Client client = connection.Client
			?? throw new InvalidOperationException("Connection client is null.");
		
		Log.Print("Listening...");
		while (!connection.Token.IsCancellationRequested)
		{
			string message = await client.ReceiveAsync(connection.Token);
			_onMessage?.Invoke(message);
		}
	}

	private static async Task<bool> RetryAsync(Connection connection)
	{
		Log.Print($"Retrying in {ConnectionRetryDelay / 1000} sec...");
		try
		{
			await Task.Delay(ConnectionRetryDelay, connection.Token);
			return true;
		}
		catch (OperationCanceledException)
		{
			return false;
		}
	}

	private async Task DisconnectAsync(Connection connection)
	{		
		if (connection.Client != null)
		{
			try
			{
				await connection.Client.CloseAsync(CancellationToken.None);
			}
			catch
			{}
			connection.Client.Dispose();
			connection.Client = null;
		}

		if (connection.Connected)
		{
			connection.Connected = false;
			_onDisconnect?.Invoke();
		}
	}
}
