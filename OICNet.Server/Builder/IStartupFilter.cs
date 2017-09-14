using System;

namespace OICNet.Server.Builder
{
    public interface IStartupFilter
    {
        Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next);
    }
}