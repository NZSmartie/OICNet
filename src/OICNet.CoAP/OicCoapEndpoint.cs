using System;
using System.Threading.Tasks;
using CoAPNet;

namespace OICNet.CoAP
{
    public class OicCoapEndpoint : ICoapEndpoint, IOicEndpoint
    {
        private readonly ICoapEndpoint _coapEndpoint;

        bool IOicEndpoint.IsSecure => _coapEndpoint.IsSecure;

        bool ICoapEndpoint.IsSecure => _coapEndpoint.IsSecure;

        public string Authority => BaseUri.Authority;

        public bool IsMulticast => _coapEndpoint.IsMulticast;

        public Uri BaseUri => _coapEndpoint.BaseUri;

        public IOicTransport Transport { get; internal set; }

        public Task SendAsync(CoapPacket packet) 
            => _coapEndpoint.SendAsync(packet);
        
        public Task<CoapPacket> ReceiveAsync()
            => _coapEndpoint.ReceiveAsync();

        public OicCoapEndpoint(ICoapEndpoint coapEndpoint)
        {
            _coapEndpoint = coapEndpoint;
        }

        public void Dispose()
        {
            _coapEndpoint.Dispose();
        }
    }
}