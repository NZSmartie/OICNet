using System;
using System.Threading.Tasks;

using CoAPNet;
using System.Threading;
using System.Text;

namespace OICNet.CoAP
{
    public class OicCoapTransport : IOicTransport
    {
        private readonly CoapClient _client;

        public OicCoapTransport(CoapClient client)
        {
            _client = client;
        }

        public OicCoapTransport(ICoapEndpoint endpoint) 
        {
            _client = new CoapClient(endpoint);
        }

        public async Task BroadcastMessageAsync(OicMessage request)
        {
            var message = request.ToCoapMessage();
            message.IsMulticast = true;
            message.Type = CoapMessageType.NonConfirmable;

            await _client.SendAsync(message);
        }

        public async Task<OicReceivedMessage> ReceiveMessageAsync(CancellationToken token)
        {
            var message = await _client.ReceiveAsync(token);

            if (message.Message.Code.IsServerError())
                throw new CoapException(Encoding.UTF8.GetString(message.Message.Payload), message.Message.Code);

            return new OicReceivedMessage
            {
                Endpoint = new OicCoapEndpoint(message.Endpoint),
                Message = message.Message.Code.IsRequest()
                    ? (OicMessage)message.Message.ToOicRequest()
                    : (OicMessage)message.Message.ToOicResponse()
            };
        }

        public async Task<int> SendMessageAsync(OicMessage request, IOicEndpoint endpoint = null)
        {
            var message = request.ToCoapMessage();
            var coapEndpoint = endpoint as OicCoapEndpoint;
            if (coapEndpoint == null && endpoint != null)
                throw new InvalidOperationException();

            return await _client.SendAsync(message, coapEndpoint);
        }
    }
}
