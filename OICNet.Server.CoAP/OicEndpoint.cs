using System;
using System.Threading.Tasks;
using CoAPNet;

namespace OICNet.Server.CoAP
{
    public class OicEndpoint : ICoapEndpoint, IOicEndpoint
    {
        private readonly ICoapEndpoint _coapEndpoint;

        public IOicTransport Transport { get; set; }

        bool IOicEndpoint.IsSecure => _coapEndpoint.IsSecure;

        bool ICoapEndpoint.IsSecure => _coapEndpoint.IsSecure;

        public string Authority => BaseUri.Authority;

        public bool IsMulticast => _coapEndpoint.IsMulticast;

        public Uri BaseUri => _coapEndpoint.BaseUri;

        public void Dispose()
        {
            _coapEndpoint.Dispose();
        }

        public Task SendAsync(CoapPacket packet)
        {
            throw new NotImplementedException();
        }

        public Task<CoapPacket> ReceiveAsync()
        {
            throw new NotImplementedException();
        }


        public OicEndpoint(ICoapEndpoint coapEndpoint)
        {
            _coapEndpoint = coapEndpoint;
        }
    }
}