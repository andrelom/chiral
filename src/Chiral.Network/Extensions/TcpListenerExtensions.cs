using System.Net.Sockets;

namespace Chiral.Network.Extensions;

public static class TcpListenerExtensions
{
    public static async Task<TcpClient> AcceptTcpClientAsync(this TcpListener listener, CancellationToken token)
    {
        try
        {
            return await listener.AcceptTcpClientAsync(token);
        }
        catch (SocketException ex) when (ex.SocketErrorCode == SocketError.Interrupted)
        {
            throw new OperationCanceledException();
        }
        catch (ObjectDisposedException) when (token.IsCancellationRequested)
        {
            throw new OperationCanceledException();
        }
    }
}
