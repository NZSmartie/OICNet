namespace OICNet.Server.Mvc
{
    public class ActionContext
    {
        public OicContext OicContext { get; }

        public ActionContext()
        {
            OicContext = new OicContext();
        }

        public ActionContext(ActionContext actionContext)
            : this(actionContext.OicContext)
        { }

        public ActionContext(OicContext oicContext)
        {
            OicContext = oicContext;
        }
    }
}