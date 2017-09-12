using System;
using System.Linq;
using CoAPNet;

namespace OICNet.Server.CoAP.Utils
{
    public static class Extensions
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

        public static CoapMessage ToCoapMessage(this OicResponse response)
        {
            var message = new CoapMessage
            {
                Code = response.ResposeCode.ToCoapMessageCode(),
                Payload = response.Content,
            };

            if(response.ToUri != null)
                message.FromUri(response.ToUri);

            if(response.ContentType != OicMessageContentType.None)
                message.Options.Add(response.ContentType.ToCoapContentFormat());

            return message;
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
            throw new NotSupportedException();
        }

        public static CoAPNet.Options.ContentFormat ToCoapContentFormat(this OicMessageContentType contentType)
        {
            var contentFormat = new CoAPNet.Options.ContentFormat();
            switch (contentType)
            {
                case OicMessageContentType.ApplicationJson:
                    contentFormat.MediaType = CoAPNet.Options.ContentFormatType.ApplicationJson;
                    break;
                case OicMessageContentType.ApplicationCbor:
                    contentFormat.MediaType = CoAPNet.Options.ContentFormatType.ApplicationCbor;
                    break;
                case OicMessageContentType.ApplicationXml:
                    contentFormat.MediaType = CoAPNet.Options.ContentFormatType.ApplicationXml;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(contentType), contentType, null);
            }
            return contentFormat;
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
                    throw new ArgumentOutOfRangeException(nameof(code), code, null);
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
                case OicResponseCode.MethodNotAllowed:
                    return CoapMessageCode.MethodNotAllowed;
                case OicResponseCode.NotAcceptable:
                    return CoapMessageCode.NotAcceptable;
                case OicResponseCode.PreconditionFailed:
                    return CoapMessageCode.PreconditionFailed;
                case OicResponseCode.RequestEntityTooLarge:
                    return CoapMessageCode.RequestEntityTooLarge;
                case OicResponseCode.UnsupportedContentFormat:
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
                    throw new ArgumentOutOfRangeException(nameof(code), code, null);
            }
        }

        #endregion
    }
}