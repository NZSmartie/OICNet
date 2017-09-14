using Microsoft.Extensions.DependencyInjection;
using CoAPNet;
using CoAPNet.Udp;
using OICNet.Server.Builder;

// ReSharper disable once CheckNamespace
namespace OICNet.Server.Hosting
{
    public static class OicHostBuilderCoapUdpExtensions
    {
        public static OicHostBuilder UseCoapUdp(this OicHostBuilder builder)
        {
            return builder.ConfigureServices(services =>
                services
                    .AddSingleton<ICoapTransportFactory, CoapUdpTransportFactory>()
            );


        }
    }
}