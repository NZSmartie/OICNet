using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OICNet
{
    public class OicReceivedMessageEventArgs : EventArgs
    {
        public IOicEndpoint Endpoint;

        public OicResponse Message;
    }

    public interface IOicInterface
    {
        Task BroadcastMessageAsync(OicRequest message);

        Task SendMessageAsync(IOicEndpoint endpoint, OicRequest message);

        Task<OicResponse> SendMessageWithResponseAsync(IOicEndpoint endpoint, OicRequest message);

        event EventHandler<OicReceivedMessageEventArgs> ReceivedMessage;
    }

    public interface IOicEndpoint
    {
        bool IsSecure { get; }

        string Authority { get; }

        // ummm...?
    }

    public class OicMessage
    {
        public Uri Uri { get; set; }

        public byte[] Payload { get; set; }
    }

    public class OicRequest : OicMessage
    {
        public OicMessageMethod Method { get; set; } = OicMessageMethod.Get;

        public List<string> Filters { get; set; }

        public List<OicMessageContentType> Accepts { get; set; } = new List<OicMessageContentType>();
    }

    public class OicResponse : OicMessage
    {
        public OicMessageContentType ContentType { get; set; } = OicMessageContentType.ApplicationCbor;
    }

    public enum OicMessageContentType
    {
        ApplicationJson,
        ApplicationCbor,
        ApplicationXml,
    }

    public enum OicMessageMethod
    {
        Get,
        Post,
        Put,
        Delete,
    }
}