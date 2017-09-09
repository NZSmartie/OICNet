using System;
using System.Collections.Generic;
using CoAPNet;

namespace OICNet.Server.CoAP
{
    public class OicCoapServerOptions
    {
        public IServiceProvider ApplicationServices { get; set; }

        public IList<ICoapEndpoint> Listeners = new List<ICoapEndpoint>();

        public void Listen(ICoapEndpoint coapEndpoint)
        {
            Listeners.Add(coapEndpoint ?? throw new ArgumentNullException(nameof(coapEndpoint)));
        }
    }
}