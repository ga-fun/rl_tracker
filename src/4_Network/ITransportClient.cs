namespace GuillaumeAst.Network;

public interface ITransportClient : IDisposable
{
	Task ConnectAsync(CancellationToken token = default);
	Task<byte[]> ReceiveAsync(CancellationToken token = default);
	Task CloseAsync(CancellationToken token = default);
}
