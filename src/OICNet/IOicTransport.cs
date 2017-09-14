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
        /// <summary>
        /// The URI of the recipient of the message.
        /// </summary>
        public virtual Uri ToUri { get; set; }

        /// <summary>
        /// The URI of the message originator.
        /// </summary>
        public virtual Uri FromUri { get; set; }

        /// <summary>
        /// The identifier that uniquely identifies the message in the originator and the recipient.
        /// </summary>
        public virtual int RequestId { get; set; }

        /// <summary>
        /// Information specific to the operation.
        /// </summary>
        public virtual byte[] Content { get; set; }

        public virtual OicMessageContentType ContentType { get; set; } = OicMessageContentType.ApplicationCbor;
    }

    public class OicRequest : OicMessage
    {
        /// <summary>
        /// Specific operation requested to be performed by the Server.
        /// </summary>
        public virtual OicRequestOperation Operation { get; set; } = OicRequestOperation.Get;

        public virtual List<OicMessageContentType> Accepts { get; } = new List<OicMessageContentType>();

        /// <summary>
        /// Indicator for an observe request.
        /// </summary>
        public virtual bool Observe { get; set; }
    }

    public class OicResponse : OicMessage
    {
        /// <summary>
        /// Indicator of the result of the request; whether it was accepted and what the conclusion of the operation was.
        /// </summary>
        /// <remarks>
        /// The values of the response code for CRUDN operations shall conform to those as defined in section 5.9 and 12.1.2 in IETF RFC 7252.
        /// </remarks>
        public virtual OicResponseCode ResposeCode { get; set; }

        /// <summary>
        /// Indicator for an observe response.
        /// </summary>
        public virtual bool Observe { get; set; }
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
        OperationNotAllowed = 405,
        NotAcceptable = 406,
        PreconditionFailed = 412,
        RequestEntityTooLarge = 413,
        UnsupportedContentType = 415,
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
        None,
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