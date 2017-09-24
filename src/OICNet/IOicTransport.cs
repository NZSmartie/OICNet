using System;
using System.Threading;
using System.Threading.Tasks;

namespace OICNet
{
    public class OicReceivedMessage
    {
        public IOicEndpoint Endpoint;

        public OicMessage Message;
    }

    public interface IOicTransport
    {
        Task BroadcastMessageAsync(OicMessage message);

        Task<int> SendMessageAsync(OicMessage message, IOicEndpoint endpoint = null);

        Task<OicReceivedMessage> ReceiveMessageAsync(CancellationToken token);
    }

    public interface IOicEndpoint : IDisposable
    {
        IOicTransport Transport { get; }

        bool IsSecure { get; }

        string Authority { get; }
    }
}