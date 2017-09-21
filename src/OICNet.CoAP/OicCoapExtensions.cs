using System;
using System.Linq;

using CoAPNet;

namespace OICNet.CoAP
{
    public static class OicCoapExtensions
    {
        public static OicRequest ToOicRequest(this CoapMessage message)
        {
            var request = new OicRequest
            {
                //FromUri = TODO populate FromUri
                ToUri = message.GetUri(),

                Content = message.Payload,
                ContentType = message.Options.Get<CoAPNet.Options.ContentFormat>()?.MediaType.ToOicMessageContentType() ?? OicMessageContentType.None,
                Operation = message.Code.ToOicRequestOperation(),
                RequestId = message.Id,
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
                RequestId = message.Id,
            };

            return request;
        }

        public static CoapMessage ToCoapMessage(this OicMessage message)
        {
            var coapMessage = new CoapMessage
            {
                Payload = message.Content,
            };

            if (message.ContentType != OicMessageContentType.None)
                coapMessage.Options.Add(new CoAPNet.Options.ContentFormat(message.ContentType.ToCoapContentFormat()));

            if (message.ToUri != null)
                coapMessage.FromUri(message.ToUri);

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
            throw new OicException("Unsupported content type", OicResponseCode.UnsupportedContentType);
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
            switch (code)
            {
                case CoapMessageCode.Get:
                    return OicRequestOperation.Get;
                case CoapMessageCode.Post:
                    return OicRequestOperation.Post;
                case CoapMessageCode.Put:
                    return OicRequestOperation.Put;
                case CoapMessageCode.Delete:
                    return OicRequestOperation.Delete;
                default:
                    throw new OicException("Unsupported operation", new ArgumentOutOfRangeException(nameof(code), code, null), OicResponseCode.BadRequest);
            }
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
            switch (code)
            {
                case CoapMessageCode.Created:
                    return OicResponseCode.Created;
                case CoapMessageCode.Delete:
                    return OicResponseCode.Deleted;
                case CoapMessageCode.Valid:
                    return OicResponseCode.Valid;
                case CoapMessageCode.Changed:
                    return OicResponseCode.Changed;
                case CoapMessageCode.Content:
                    return OicResponseCode.Content;
                case CoapMessageCode.BadRequest:
                    return OicResponseCode.BadRequest;
                case CoapMessageCode.Unauthorized:
                    return OicResponseCode.Unauthorized;
                case CoapMessageCode.BadOption:
                    return OicResponseCode.BadOption;
                case CoapMessageCode.Forbidden:
                    return OicResponseCode.Forbidden;
                case CoapMessageCode.NotFound:
                    return OicResponseCode.NotFound;
                case CoapMessageCode.MethodNotAllowed:
                    return OicResponseCode.OperationNotAllowed;
                case CoapMessageCode.NotAcceptable:
                    return OicResponseCode.NotAcceptable;
                case CoapMessageCode.PreconditionFailed:
                    return OicResponseCode.PreconditionFailed;
                case CoapMessageCode.RequestEntityTooLarge:
                    return OicResponseCode.RequestEntityTooLarge;
                case CoapMessageCode.UnsupportedContentFormat:
                    return OicResponseCode.UnsupportedContentType;
                case CoapMessageCode.InternalServerError:
                    return OicResponseCode.InternalServerError;
                case CoapMessageCode.NotImplemented:
                    return OicResponseCode.NotImplemented;
                case CoapMessageCode.BadGateway:
                    return OicResponseCode.BadGateway;
                case CoapMessageCode.ServiceUnavailable:
                    return OicResponseCode.ServiceUnavailable;
                case CoapMessageCode.GatewayTimeout:
                    return OicResponseCode.GatewayTimeout;
                case CoapMessageCode.ProxyingNotSupported:
                    return OicResponseCode.ProxyingNotSupported;
                default:
                    throw new OicException("Unsupported response code", new ArgumentOutOfRangeException(nameof(code), code, null));
            }
        }

        #endregion
    }
}