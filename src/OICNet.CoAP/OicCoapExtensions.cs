using System;
using System.Linq;

using CoAPNet;

namespace OICNet.CoAP
{
    public static class OicCoapExtensions
    {
        public static int GetOicRequestId(this CoapMessage message)
            => message.Token.Length > 0 ? BitConverter.ToInt32(message.Token, 0) : 0;

        public static OicRequest ToOicRequest(this CoapMessage message)
        {
            var request = new OicRequest
            {
                //FromUri = TODO populate FromUri
                ToUri = new UriBuilder(message.GetUri()) { Scheme = "oic" }.Uri,

                Content = message.Payload,
                ContentType = message.Options.Get<CoAPNet.Options.ContentFormat>()?.MediaType.ToOicMessageContentType() ?? OicMessageContentType.None,
                Operation = message.Code.ToOicRequestOperation(),
                RequestId = (message.Token.Length > 0) ? BitConverter.ToInt32(message.Token, 0) : 0,
            };

            foreach (var oicMessageContentType in message.Options.GetAll<CoAPNet.Options.Accept>().Select(a => a.MediaType.ToOicMessageContentType()))
                request.Accepts.Add(oicMessageContentType);

            return request;
        }

        public static OicResponse ToOicResponse(this CoapMessage message)
        {
            var request = new OicResponse
            {
                FromUri = message.GetUri(),
                //ToUri = TODO populate ToUri

                Content = message.Payload,
                ContentType = message.Options.Get<CoAPNet.Options.ContentFormat>()?.MediaType.ToOicMessageContentType() ?? OicMessageContentType.None,
                ResposeCode = message.Code.ToOicResponseCode(),
                RequestId = (message.Token.Length > 0) ? BitConverter.ToInt32(message.Token, 0) : 0,
            };

            return request;
        }

        public static CoapMessage ToCoapMessage(this OicMessage message)
        {
            var coapMessage = new CoapMessage
            {
                Payload = message.Content,
                Token = BitConverter.GetBytes(message.RequestId),
            };

            if (message.ContentType != OicMessageContentType.None)
                coapMessage.Options.Add(new CoAPNet.Options.ContentFormat(message.ContentType.ToCoapContentFormat()));

            if (message.ToUri != null)
            {
                if (message.ToUri.IsAbsoluteUri)
                    coapMessage.SetUri(new UriBuilder(message.ToUri) { Scheme = "coap" }.Uri);
                else
                    coapMessage.SetUri(message.ToUri, UriComponents.PathAndQuery);
            }

            if (message is OicRequest request)
            {
                coapMessage.Code = request.Operation.ToCoapMessageCode();
                foreach (var contentType in request.Accepts)
                    coapMessage.Options.Add(new CoAPNet.Options.Accept(contentType.ToCoapContentFormat()));
            }
            else if (message is OicResponse response)
            {
                coapMessage.Code = response.ResposeCode.ToCoapMessageCode();
            }

            return coapMessage;
        }


        #region CoAP To/From Conversions 

        public static OicMessageContentType ToOicMessageContentType(this CoAPNet.Options.ContentFormatType formatType)
        {
            if (formatType == CoAPNet.Options.ContentFormatType.ApplicationCbor)
                return OicMessageContentType.ApplicationCbor;
            if (formatType == CoAPNet.Options.ContentFormatType.ApplicationJson)
                return OicMessageContentType.ApplicationJson;
            if (formatType == CoAPNet.Options.ContentFormatType.ApplicationXml)
                return OicMessageContentType.ApplicationXml;
            throw new OicException($"Unsupported content type ({formatType})", OicResponseCode.UnsupportedContentType);
        }

        public static CoAPNet.Options.ContentFormatType ToCoapContentFormat(this OicMessageContentType contentType)
        {
            switch (contentType)
            {
                case OicMessageContentType.ApplicationJson:
                    return CoAPNet.Options.ContentFormatType.ApplicationJson;
                case OicMessageContentType.ApplicationCbor:
                    return CoAPNet.Options.ContentFormatType.ApplicationCbor;
                case OicMessageContentType.ApplicationXml:
                    return CoAPNet.Options.ContentFormatType.ApplicationXml;
                default:
                    throw new OicException("Unsupported content type", new ArgumentOutOfRangeException(nameof(contentType), contentType, null));
            }
        }

        public static OicRequestOperation ToOicRequestOperation(this CoapMessageCode code)
        {
            if (code == CoapMessageCode.Get)
                return OicRequestOperation.Get;
            if (code == CoapMessageCode.Post)
                return OicRequestOperation.Post;
            if (code == CoapMessageCode.Put)
                return OicRequestOperation.Put;
            if (code == CoapMessageCode.Delete)
                return OicRequestOperation.Delete;

            throw new OicException("Unsupported operation", new ArgumentOutOfRangeException(nameof(code), code, null), OicResponseCode.BadRequest);

        }

        public static CoapMessageCode ToCoapMessageCode(this OicRequestOperation code)
        {
            switch (code)
            {
                case OicRequestOperation.Get:
                    return CoapMessageCode.Get;
                case OicRequestOperation.Post:
                    return CoapMessageCode.Post;
                case OicRequestOperation.Put:
                    return CoapMessageCode.Put;
                case OicRequestOperation.Delete:
                    return CoapMessageCode.Delete;
                default:
                    throw new OicException("Unsupported operation", new ArgumentOutOfRangeException(nameof(code), code, null), OicResponseCode.BadRequest);
            }
        }

        public static CoapMessageCode ToCoapMessageCode(this OicResponseCode code)
        {
            switch (code)
            {
                case OicResponseCode.Created:
                    return CoapMessageCode.Created;
                case OicResponseCode.Deleted:
                    return CoapMessageCode.Deleted;
                case OicResponseCode.Valid:
                    return CoapMessageCode.Valid;
                case OicResponseCode.Changed:
                    return CoapMessageCode.Changed;
                case OicResponseCode.Content:
                    return CoapMessageCode.Content;
                case OicResponseCode.BadRequest:
                    return CoapMessageCode.BadRequest;
                case OicResponseCode.Unauthorized:
                    return CoapMessageCode.Unauthorized;
                case OicResponseCode.BadOption:
                    return CoapMessageCode.BadOption;
                case OicResponseCode.Forbidden:
                    return CoapMessageCode.Forbidden;
                case OicResponseCode.NotFound:
                    return CoapMessageCode.NotFound;
                case OicResponseCode.OperationNotAllowed:
                    return CoapMessageCode.MethodNotAllowed;
                case OicResponseCode.NotAcceptable:
                    return CoapMessageCode.NotAcceptable;
                case OicResponseCode.PreconditionFailed:
                    return CoapMessageCode.PreconditionFailed;
                case OicResponseCode.RequestEntityTooLarge:
                    return CoapMessageCode.RequestEntityTooLarge;
                case OicResponseCode.UnsupportedContentType:
                    return CoapMessageCode.UnsupportedContentFormat;
                case OicResponseCode.InternalServerError:
                    return CoapMessageCode.InternalServerError;
                case OicResponseCode.NotImplemented:
                    return CoapMessageCode.NotImplemented;
                case OicResponseCode.BadGateway:
                    return CoapMessageCode.BadGateway;
                case OicResponseCode.ServiceUnavailable:
                    return CoapMessageCode.ServiceUnavailable;
                case OicResponseCode.GatewayTimeout:
                    return CoapMessageCode.GatewayTimeout;
                case OicResponseCode.ProxyingNotSupported:
                    return CoapMessageCode.ProxyingNotSupported;
                default:
                    throw new OicException("Unsupported response code", new ArgumentOutOfRangeException(nameof(code), code, null));
            }
        }

        public static OicResponseCode ToOicResponseCode(this CoapMessageCode code)
        {
            if (code == CoapMessageCode.Created)
                return OicResponseCode.Created;
            if (code == CoapMessageCode.Delete)
                return OicResponseCode.Deleted;
            if (code == CoapMessageCode.Valid)
                return OicResponseCode.Valid;
            if (code == CoapMessageCode.Changed)
                return OicResponseCode.Changed;
            if (code == CoapMessageCode.Content)
                return OicResponseCode.Content;
            if (code == CoapMessageCode.BadRequest)
                return OicResponseCode.BadRequest;
            if (code == CoapMessageCode.Unauthorized)
                return OicResponseCode.Unauthorized;
            if (code == CoapMessageCode.BadOption)
                return OicResponseCode.BadOption;
            if (code == CoapMessageCode.Forbidden)
                return OicResponseCode.Forbidden;
            if (code == CoapMessageCode.NotFound)
                return OicResponseCode.NotFound;
            if (code == CoapMessageCode.MethodNotAllowed)
                return OicResponseCode.OperationNotAllowed;
            if (code == CoapMessageCode.NotAcceptable)
                return OicResponseCode.NotAcceptable;
            if (code == CoapMessageCode.PreconditionFailed)
                return OicResponseCode.PreconditionFailed;
            if (code == CoapMessageCode.RequestEntityTooLarge)
                return OicResponseCode.RequestEntityTooLarge;
            if (code == CoapMessageCode.UnsupportedContentFormat)
                return OicResponseCode.UnsupportedContentType;
            if (code == CoapMessageCode.InternalServerError)
                return OicResponseCode.InternalServerError;
            if (code == CoapMessageCode.NotImplemented)
                return OicResponseCode.NotImplemented;
            if (code == CoapMessageCode.BadGateway)
                return OicResponseCode.BadGateway;
            if (code == CoapMessageCode.ServiceUnavailable)
                return OicResponseCode.ServiceUnavailable;
            if (code == CoapMessageCode.GatewayTimeout)
                return OicResponseCode.GatewayTimeout;
            if (code == CoapMessageCode.ProxyingNotSupported)
                return OicResponseCode.ProxyingNotSupported;

            throw new OicException("Unsupported response code", new ArgumentOutOfRangeException(nameof(code), code, null));

        }

        #endregion
    }
}