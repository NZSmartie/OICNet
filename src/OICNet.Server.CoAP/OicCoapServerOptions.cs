using System;
using System.Collections.Generic;
using CoAPNet;

namespace OICNet.Server.CoAP
{
    public class OicCoapServerOptions
    {
        public IServiceProvider HostingServices { get; set; }

        public IList<ICoapEndpoint> Listeners = new List<ICoapEndpoint>();

        public bool UseCoreLink { get; set; }

        public void Listen(ICoapEndpoint coapEndpoint)
        {
            Listeners.Add(coapEndpoint ?? throw new ArgumentNullException(nameof(coapEndpoint)));
        }
    }
}