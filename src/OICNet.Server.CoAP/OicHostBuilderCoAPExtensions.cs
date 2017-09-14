using System;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using OICNet.Server.Builder;
using OICNet.Server.CoAP;

// ReSharper disable once CheckNamespace
namespace OICNet.Server.Hosting
{
    public static class OicHostBuilderCoapExtensions
    {
        public static OicHostBuilder UseCoap(this OicHostBuilder builder)
        {
            return builder.ConfigureServices(services =>
                services
                    .AddTransient<IConfigureOptions<OicCoapServerOptions>, OicCoapServerOptionsSetup>()
                    .AddTransient<IOicContextFactory, OicCoapContextFactory>()
                    .AddSingleton<IOicServer, OicCoapServer>()
            );
        }

        public static OicHostBuilder UseCoap(this OicHostBuilder builder, Action<OicCoapServerOptions> options)
        {
            return builder.UseCoap().ConfigureServices(services =>
            {
                services.Configure(options);
            });
        }
    }
}