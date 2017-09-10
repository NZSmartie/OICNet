using OICNet.Server.Mvc;

namespace OICNet.Server
{
    public class StatusCodeResult : ActionResult
    {
        public OicResponseCode ResponseCode { get; set; }
        public string Message { get; set; }

        public StatusCodeResult(OicResponseCode responseCode, string message = default(string))
        {
            ResponseCode = responseCode;
            Message = message;
        }

        public override void ExecuteResult(ActionContext context)
        {
            context.OicContext.Response.ResposeCode = ResponseCode;
        }
    }
}