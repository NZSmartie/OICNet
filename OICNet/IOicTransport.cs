using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OICNet
{
    public class OicReceivedMessageEventArgs : EventArgs
    {
        public IOicEndpoint Endpoint;
        public OicMessage Message;
    }

    public interface IOicInterface
    {
        Task BroadcastMessageAsync(OicMessage message);

        Task SendMessageAsync(IOicEndpoint endpoint, OicMessage message);

        event EventHandler<OicReceivedMessageEventArgs> ReceivedMessage;
    }

    public interface IOicEndpoint
    {
        bool IsSecure { get; }

        string Authority { get; }

        // ummm...?
    }

    // TODO: create Request/Response sub-classes?
    public class OicMessage
    {
        public OicMessageMethod Method { get; set; } = OicMessageMethod.Get;

        public List<string> Filters { get; set; }

        public string Uri { get; set; }

        public List<OicMessageContentType> Accepts { get; set; } = new List<OicMessageContentType>();

        public OicMessageContentType ContentType { get; set; } = OicMessageContentType.ApplicationCbor;

        public byte[] Payload { get; set; }
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