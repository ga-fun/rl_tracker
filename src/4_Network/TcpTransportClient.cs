using System.Net;
using System.Net.Sockets;

namespace GuillaumeAst.Network;

public sealed class TcpTransportClient(IPAddress address, int port) : ITransportClient
{
	private const int BufferSize = 8192;
	private readonly IPEndPoint _endpoint = new(address, port);
	private readonly TcpClient _client = new();
	private NetworkStream? _stream = null;

	public async Task ConnectAsync(CancellationToken token = default)
	{
		await _client.ConnectAsync(_endpoint, token);
		_stream = _client.GetStream();
		CheckConnection();
	}

	public async Task<byte[]> ReceiveAsync(CancellationToken token = default)
	{
		CheckConnection();
		byte[] buffer = new byte[BufferSize];
		int count = await _stream!.ReadAsync(buffer, token);

		if (count == 0)
		{
			throw new IOException("TCP connection closed");
		}
		return buffer[..count];
	}

	private void CheckConnection()
	{
		if (_stream == null || !_client.Connected)
		{
			throw new IOException("TCP connection is not open");
		}
	}

	public Task CloseAsync(CancellationToken token = default)
	{
		Dispose();
		return Task.CompletedTask;
	}

	public void Dispose()
	{
		_stream?.Dispose();
		_stream = null;
		_client.Dispose();
	}
}
