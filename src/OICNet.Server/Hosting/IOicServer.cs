using System;
using OICNet.Server.Builder;

namespace OICNet.Server.Hosting
{
    public interface IOicServer : IDisposable
    {
        void Start(OicHostApplication application);
    }
}