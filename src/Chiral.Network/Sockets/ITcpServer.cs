namespace Chiral.Network.Sockets;

public interface ITcpServer : IDisposable
{
    Task StartAsync(TcpConnectionHandler handler, CancellationToken token);

    Task StopAsync(CancellationToken token);
}
