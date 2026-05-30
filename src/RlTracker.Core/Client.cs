using System.Text;
using System.Net;
using System.Net.WebSockets;

namespace RlTracker.Core;

internal sealed class Client(int port) : IDisposable
{
	private const int BufferSize = 8192;
	private readonly ClientWebSocket _socket = new();
	private readonly Uri _uri = new($"ws://{IPAddress.Loopback}:{port}");

	internal async Task ConnectAsync(CancellationToken token = default)
	{
		await _socket.ConnectAsync(_uri, token);
		CheckConnection();
	}

	internal async Task<string> ReceiveAsync(CancellationToken token = default)
	{
		byte[] buffer = new byte[BufferSize];
		WebSocketReceiveResult result;

		CheckConnection();
		while (true)
		{
			using MemoryStream stream = new();
			do
			{
				result = await _socket.ReceiveAsync(buffer, token);
				if (result.MessageType == WebSocketMessageType.Close)
					throw new WebSocketException("WebSocket was closed.");
				if (result.MessageType == WebSocketMessageType.Text)
					stream.Write(buffer, 0, result.Count);
			}
			while (!result.EndOfMessage);

			if (result.MessageType == WebSocketMessageType.Text)
				return Encoding.UTF8.GetString(stream.ToArray());
			else
				Console.WriteLine($"{Log.Yellow}Binary message ignored.{Log.Reset}");
		}
	}

	internal async Task CloseAsync(CancellationToken token = default)
	{
		if (_socket.State == WebSocketState.Open
			|| _socket.State == WebSocketState.CloseReceived)
		{
			await _socket.CloseAsync(
				WebSocketCloseStatus.NormalClosure,
				"Closing",
				token
			);
		}
	}

	private void CheckConnection()
	{
		if (_socket.State != WebSocketState.Open)
			throw new WebSocketException("WebSocket is not open.");
	}

	public void Dispose()
	{
		_socket.Dispose();
	}
}
