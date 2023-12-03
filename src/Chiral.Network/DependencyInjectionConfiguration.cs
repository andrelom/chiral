using Chiral.Core.Extensions;
using Chiral.Network.Options;
using Chiral.Network.Sockets;
using Chiral.Network.Transport;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Chiral.Network;

public static class DependencyInjectionConfiguration
{
    public static IServiceCollection AddChiralNetwork(this IServiceCollection services)
    {
        // Singleton: IPeer
        services.AddSingleton(AddPeer);

        // Scoped: ITcpServer
        services.AddScoped<ITcpServer, TcpServer>();

        // Hosted Service: Server
        services.AddHostedService<Server>();

        return services;
    }

    #region Private Methods: Add

    private static IPeer AddPeer(IServiceProvider provider)
    {
        var configuration = provider.GetService<IConfiguration>();
        var options = configuration?.Load<PeerOptions>();

        if (options == null)
        {
            throw new InvalidOperationException();
        }

        return new Peer(options.Host, options.Port);
    }

    #endregion
}
