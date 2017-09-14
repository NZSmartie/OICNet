using System;
using System.Runtime.Serialization;

namespace OICNet
{
    public class OicException : Exception
    {
        public OicResponseCode ResponseCode { get; }

        public OicException()
        { }

        public OicException(string message) 
            : base(message)
        { ResponseCode = OicResponseCode.InternalServerError; }

        public OicException(string message, OicResponseCode code)
            : base(message)
        { ResponseCode = code; }

        public OicException(string message, Exception innerException) 
            : base(message, innerException)
        { ResponseCode = OicResponseCode.InternalServerError; }

        public OicException(string message, Exception innerException, OicResponseCode code)
            : base(message, innerException)
        { ResponseCode = code; }
    }
}
