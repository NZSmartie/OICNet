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

    public interface IOicTransport
    {
        Task BroadcastMessageAsync(OicRequest message);

        Task SendMessageAsync(IOicEndpoint endpoint, OicMessage message);

        Task<OicResponse> SendMessageWithResponseAsync(IOicEndpoint endpoint, OicMessage message);

        event EventHandler<OicReceivedMessageEventArgs> ReceivedMessage;
    }

    public interface IOicEndpoint
    {
        IOicTransport Transport { get; }

        bool IsSecure { get; }

        string Authority { get; }

        // ummm...?
    }

    public class OicMessage
    {
        public Uri ToUri { get; set; }

        public Uri FromUri { get; set; }

        public int RequestId { get; set; }

        public byte[] Content { get; set; }

        public OicMessageContentType ContentType { get; set; } = OicMessageContentType.ApplicationCbor;

    }

    public class OicRequest : OicMessage
    {
        public OicRequestOperation Operation { get; set; } = OicRequestOperation.Get;

        public List<OicMessageContentType> Accepts { get; } = new List<OicMessageContentType>();
    }

    public class OicResponse : OicMessage
    {
        public OicResponseCode ResposeCode;
    }

    public enum OicResponseCode
    {
        // 2.xx Success
        Created = 201,
        Deleted = 202,
        Valid = 203,
        Changed = 204,
        Content = 205,
        // 4.xx Client Error
        BadRequest = 400,
        Unauthorized = 401,
        BadOption = 402,
        Forbidden = 403,
        NotFound = 404,
        MethodNotAllowed = 405,
        NotAcceptable = 406,
        PreconditionFailed = 412,
        RequestEntityTooLarge = 413,
        UnsupportedContentFormat = 415,
        // 5.xx Server Error
        InternalServerError = 500,
        NotImplemented = 501,
        BadGateway = 502,
        ServiceUnavailable = 503,
        GatewayTimeout = 504,
        ProxyingNotSupported = 505
    }

    public enum OicMessageContentType
    {
        ApplicationJson,
        ApplicationCbor,
        ApplicationXml,
    }

    public enum OicRequestOperation
    {
        Get,
        Post,
        Put,
        Delete,
    }
}