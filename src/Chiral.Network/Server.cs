using System.Net.Sockets;
using System.Text;
using Chiral.Network.Sockets;
using Chiral.Network.Transport;
using Chiral.Protocol;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Chiral.Network;

public class Server : IHostedService, IDisposable
{
    private bool _disposed;

    private readonly IHostApplicationLifetime _lifetime;

    private readonly IPeer _peer;

    private readonly ITcpServer _server;

    private readonly ILogger<Server> _logger;

    private readonly IRoutingTable<IPeer> _routing;

    public Server(
        IPeer peer,
        ITcpServer server,
        IHostApplicationLifetime lifetime,
        ILogger<Server> logger)
    {
        _peer = peer;
        _server = server;
        _lifetime = lifetime;
        _logger = logger;

        _routing = new RoutingTable<IPeer>(peer);
    }

    public async Task StartAsync(CancellationToken token)
    {
        _logger.LogInformation("Server is listening on {0}:{1}", _peer.Host, _peer.Port);

        try
        {
            await _server.StartAsync(HandleClientAsync, token);
        }
        catch (Exception ex)
        {
            if (ex is not OperationCanceledException)
            {
                _logger.LogError(ex.Message);
            }

            _lifetime.StopApplication();
        }
    }

    public async Task StopAsync(CancellationToken token)
    {
        await _server.StopAsync(token);
    }

    public void Dispose()
    {
        Dispose(true);

        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        _logger.LogInformation("Server is shutting down...");

        if (disposing)
        {
            _server?.Dispose();
        }

        _disposed = true;
    }

    #region Private Methods

    private async Task HandleClientAsync(TcpClient client, CancellationToken token)
    {
        // TODO: Implement the handler.

        await using var stream = client.GetStream();
        using var reader = new StreamReader(stream, Encoding.UTF8);

        var message = await Message.DeserializeAsync(reader, token);

        _logger.LogInformation($"Message Received: {message?.Serialize()}");

        await Task.Yield();
    }

    #endregion
}
