using System.Net;
using System.Net.Sockets;
using System.Text;
using GuillaumeAst.Utils;

namespace GuillaumeAst.Network;

// TODO: TCP is a byte stream, so one read can contain partial/multiple JSON events.

internal sealed class Client(int port) : IDisposable
{
	private const int BufferSize = 8192;
	private readonly TcpClient _client = new();
	private NetworkStream? _stream = null;
	private readonly IPEndPoint _endpoint = new(IPAddress.Loopback, port);

	internal async Task ConnectAsync(CancellationToken token = default)
	{
		await _client.ConnectAsync(_endpoint, token);
		_stream = _client.GetStream();
		CheckConnection();
	}

	internal async Task<string> ReceiveAsync(CancellationToken token = default)
	{
		NetworkStream stream = _stream
			?? throw new IOException("TCP stream is null");
		byte[] buffer = new byte[BufferSize];
		int count = await stream.ReadAsync(buffer, token);
		if (count == 0)
		{
			throw new IOException("TCP connection closed");
		}
		return Encoding.UTF8.GetString(buffer, 0, count);
	}

	internal void Close()
	{
		_client.Close();
	}

	private void CheckConnection()
	{
		if (!_client.Connected || _stream == null)
		{
			throw new IOException("TCP connection is not open");
		}
	}

	public void Dispose()
	{
		_stream?.Dispose();
		_client.Dispose();
	}
}
