using System;
using System.Text;

namespace OICNet.Utilities
{
    public static class OicResponseUtility
    {
        public static OicResponse CreateMessage(OicResponseCode code, string message)
        {
            return new OicResponse
            {
                ResposeCode = code,
                ContentType = OicMessageContentType.ApplicationJson,
                Content = Encoding.UTF8.GetBytes($"{{\"message\": \"{message}\"}}")
            };
        }

        public static OicResponse FromException(Exception exception)
        {
            var result = CreateMessage(OicResponseCode.InternalServerError, exception.Message);

            switch (exception)
            {
                case OicException oicEx:
                    result.ResposeCode = oicEx.ResponseCode;
                    break;
                case NotImplementedException _:
                    result.ResposeCode = OicResponseCode.NotImplemented;
                    break;
            }
            return result;
        }
    }
}
