using System;

namespace OICNet.Server.Mvc
{
    public class ContentResult : ActionResult
    {
        public IOicResource OicResource { get; }
        public OicResponseCode ResponseCode { get; }

        public ContentResult(IOicResource oicResource, OicResponseCode responseCode = OicResponseCode.Content)
        {
            OicResource = oicResource;
            ResponseCode = responseCode;
        }

        public override void ExecuteResult(ActionContext context)
        {
            context.OicContext.Response.ResposeCode = OicResponseCode.Content;
            throw new NotImplementedException();
        }
    }
}