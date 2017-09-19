using System;
using Microsoft.Extensions.Options;

namespace OICNet.Server.CoAP
{
    public class OicCoapServerOptionsSetup : IConfigureOptions<OicCoapServerOptions>
    {
        private readonly IServiceProvider _services;

        public OicCoapServerOptionsSetup(IServiceProvider services)
        {
            _services = services;
        }

        public void Configure(OicCoapServerOptions options)
        {
            options.HostingServices = _services;
        }
    }
}