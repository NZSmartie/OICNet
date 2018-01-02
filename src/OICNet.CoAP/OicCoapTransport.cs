using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using CoAPNet;
using System.Threading;
using System.Text;

namespace OICNet.CoAP
{
    public class OicCoapTransport : IOicTransport
    {
        private readonly CoapClient _client;

        private readonly Dictionary<int, CoapMessage> _requestMessages = new Dictionary<int, CoapMessage>();

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

            // Transparently read in the entire blockwise message
            // TODO: expose this API to allow sending and receivign larger bodies of data.
            var block2 = message.Message.Options.Get<CoAPNet.Options.Block2>();
            if (block2 != null)
            {
                if(!_requestMessages.TryGetValue(message.Message.GetOicRequestId(), out var baseRequest))
                    throw new OicException("Can not read block-wise stream without orignal request message");

                var ms = new MemoryStream();
                using (var reader = new CoapBlockStream(_client, message.Message, baseRequest, message.Endpoint))
                    reader.CopyTo(ms);

                message.Message.Payload = ms.ToArray();
            }

            return new OicReceivedMessage
            {
                Endpoint = new OicCoapEndpoint(message.Endpoint, this),
                Message = message.Message.Code.IsRequest()
                    ? (OicMessage)message.Message.ToOicRequest()
                    : (OicMessage)message.Message.ToOicResponse()
            };
        }

        public async Task<int> SendMessageAsync(OicMessage request, IOicEndpoint endpoint = null)
        {
            var coapEndpoint = endpoint as OicCoapEndpoint
                               ?? throw new ArgumentException($"{nameof(endpoint)} is not of type {nameof(OicCoapEndpoint)}", nameof(endpoint));

            var message = request.ToCoapMessage();

            var baseRequest = message.Clone();
            baseRequest.Payload = null;
            _requestMessages.Add(request.RequestId, baseRequest);


            return (await _client.SendAsync(message, coapEndpoint._coapEndpoint)).Id;
        }

        public override string ToString()
        {
            return $"<{GetType().Name}: {_client.Endpoint}>";
        }
    }
}
