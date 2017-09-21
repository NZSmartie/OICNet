using System;
using CoAPNet;
using OICNet.CoAP;
using OICNet.Server.Hosting;

namespace OICNet.Server.CoAP
{
    public class OicCoapContext : Tuple<ICoapConnectionInformation, CoapMessage>
    {
        public ICoapConnectionInformation ConnectionInformation => Item1;
        public CoapMessage Message => Item2;

        public OicCoapContext(ICoapConnectionInformation item1, CoapMessage item2) 
            : base(item1, item2)
        { }
    }

    public class OicCoapContextFactory : IOicContextFactory<OicCoapContext>
    {
        public OicContext CreateContext(OicCoapContext message)
        {
            return new OicContext(message.Message.ToOicRequest(),
                new ConnectionInfo
                {
                    RemoteEndpoint = new OicCoapEndpoint(message.ConnectionInformation.RemoteEndpoint),
                    LocalEndpoint = new OicCoapEndpoint(message.ConnectionInformation.LocalEndpoint)
                });
        }

        public void DisposeContext(OicContext context)
        {
            // TODO: Do we need to ensure response is sent?
        }
    }
}