using System;
using System.Threading;
using CoAPNet;
using CoAPNet.Server;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OICNet.Server.CoAP.Internal;
using OICNet.Server.Hosting;

namespace OICNet.Server.CoAP
{
    public class OicCoapServer : CoapServer, IOicServer
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly OicCoapServerOptions _options;

        public OicCoapServer(IOptions<OicCoapServerOptions> options, ICoapTransportFactory transportFactory, ILoggerFactory loggerFactory)
            : base(transportFactory, loggerFactory.CreateLogger<CoapServer>())
        {
            _loggerFactory = loggerFactory;
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public void Start(OicHostApplication application)
        {
            // TODO: Bind to endpoints with the appropiate ICoapTransportFactory
            foreach (var listener in _options.Listeners)
                BindTo(listener).Wait();

            StartAsync(new OicCoapHandler(application, _loggerFactory?.CreateLogger<OicCoapHandler>(), _options), CancellationToken.None).Wait();
        }

        public void Dispose()
        {
            StopAsync(CancellationToken.None).Wait();
        }
    }
}