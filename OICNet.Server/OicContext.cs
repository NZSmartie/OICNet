using System;

namespace OICNet.Server
{
    public class OicContext
    {
        public IServiceProvider RequestServices { get; set; }

        public ConnectionInfo Connection { get; }

        public OicResponse Response { get; set; } = new OicResponse();

        public OicRequest Request { get; }

        internal OicContext()
            : this(null, null)
        { }

        public OicContext(OicRequest request, ConnectionInfo connection)
        {
            //TODO: verify request has minimum parameters set to prevent null refernce exceptions
            Request = request;
            Connection = connection;
        }
    }
}