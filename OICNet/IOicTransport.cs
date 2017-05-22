using System.Collections.Generic;
using System.Threading.Tasks;

namespace OICNet
{
    public interface IOicTransportProvider
    {
        List<IOicBroadcaster> GetBroadcasters();
    }

    public interface IOicBroadcaster
    {
        Task SendBroadcastAsync(OicMessage message);
    }

    public interface IOicEndpoint
    {
        Task SendMessageAsync(OicMessage message);

        Task<OicMessage> ReceiveMessageAsync();
    }

    public class OicMessage
    {
        public Method Method { get; set; } = Method.Get;

        public List<string> Filters { get; set; }

        public string Uri { get; set; }

        public MimeType MimeType { get; set; } = MimeType.TextPlain;

        public byte[] Payload { get; set; }
    }

    public enum MimeType
    {
        TextPlain,
        ApplicationJson,
        AppplicationCbor,
    }

    public enum Method
    {
        Get,
        Post,
        Put,
        Delete,
        Update
    }
}