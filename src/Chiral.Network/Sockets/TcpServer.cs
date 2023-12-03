using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using Chiral.Core.Utilities;
using Chiral.Network.Transport;
using Microsoft.Extensions.Logging;

namespace Chiral.Network.Sockets;

public delegate Task TcpConnectionHandler(TcpClient client, CancellationToken token);

public class TcpServer : ITcpServer
{
    private bool _disposed;

    private bool _started;

    private TcpListener? _listener;

    private CancellationTokenSource? _cancellation;

    private readonly IPeer _peer;

    private readonly ILogger<TcpServer> _logger;

    private readonly SemaphoreSlim _semaphore = new (Constants.TcpServerSemaphoreLimit);

    private readonly ConcurrentDictionary<Guid, IEnumerable<IDisposable>> _disposables = new();

    public TcpServer(IPeer peer, ILogger<TcpServer> logger)
    {
        _peer = peer;
        _logger = logger;
    }

    public async Task StartAsync(TcpConnectionHandler handler, CancellationToken token)
    {
        if (_disposed)
        {
            return;
        }

        if (_started)
        {
            throw new InvalidOperationException("The listener is already started.");
        }

        _started = true;

        // Creates the main cancellation token source.
        _cancellation = CancellationTokenUtility.CreateSource(token);

        // Initialize all the listener resources.
        _listener = new TcpListener(IPAddress.Parse(_peer.Host), _peer.Port);

        // Accepts IPv4 and IPv6.
        _listener.Server.DualMode = true;

        // Start listening for incoming connections.
        _listener.Start();

        try
        {
            await AcceptAsync(handler, _cancellation.Token);
        }
        catch (Exception ex)
        {
            if (ex is not OperationCanceledException)
            {
                _logger.LogError(ex.Message);
            }
        }
        finally
        {
            await StopAsync(_cancellation.Token);
        }
    }

    public Task StopAsync(CancellationToken token)
    {
        if (_disposed || !_started)
        {
            return Task.CompletedTask;
        }

        _listener?.Stop();

        _cancellation?.Cancel();

        _cancellation?.Dispose();

        _listener = null;

        _cancellation = null;

        _started = false;

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        Dispose(true);

        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed || !disposing)
        {
            return;
        }

        try
        {
            _listener?.Stop();

            _cancellation?.Cancel();

            _cancellation?.Dispose();

            _semaphore.Dispose();
        }
        finally
        {
            _disposed = true;
        }
    }

    #region Private Methods

    private async Task AcceptAsync(TcpConnectionHandler handler, CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            if (_disposed || !_started || _listener == null)
            {
                break;
            }

            TcpClient client;

            await _semaphore.WaitAsync(token);

            try
            {
                // We use ConfigureAwait(false) to avoid unnecessary synchronization context
                // capture, especially in library code.
                client = await _listener.AcceptTcpClientAsync(token).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (ex is not OperationCanceledException)
                {
                    _logger.LogError(ex.Message);
                }

                continue;
            }
            finally
            {
                _semaphore.Release();
            }

            TryScheduleTask(client, handler, token);
        }
    }

    private async Task HandleAsync(Guid id, TcpClient client, TcpConnectionHandler handler, CancellationToken token)
    {
        using (client)
        {
            var remote = client.Client.RemoteEndPoint?.ToString() ?? "N/A";

            _logger.LogInformation("TCP Client ({0}) has established a connection", remote);

            try
            {
                await handler(client, token);
            }
            catch (OperationCanceledException)
            {
                _logger.LogError("TCP Client ({0}) operation timed out", remote);
            }
            catch (Exception ex)
            {
                _logger.LogError("TCP Client ({0}) unexpected error: {1}", remote, ex.Message);
            }
            finally
            {
                _logger.LogInformation("TCP Client ({0}) connection closed", remote);
            }
        }

        TryDisposeTask(id);
    }

    private void TryScheduleTask(TcpClient client, TcpConnectionHandler handler, CancellationToken token)
    {
        var id = Guid.NewGuid();
        var timeout = CancellationTokenUtility.CreateTimeoutSource(Constants.TcpServerConnectionTimeout, token);
        var task = Task.Run(() => HandleAsync(id, client, handler, timeout.Token), token);

        _disposables.TryAdd(id, new IDisposable[] { timeout, task });
    }

    private void TryDisposeTask(Guid id)
    {
        if (!_disposables.TryGetValue(id, out var disposables))
        {
            return;
        }

        foreach (var disposable in disposables)
        {
            disposable.Dispose();
        }

        _disposables.TryRemove(id, out _);
    }

    #endregion
}
