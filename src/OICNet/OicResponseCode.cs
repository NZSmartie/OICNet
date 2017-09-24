namespace OICNet
{
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
}