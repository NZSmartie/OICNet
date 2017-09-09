using System;
using OICNet.Server.Hosting;

namespace OICNet.Server.Builder
{
    public interface IApplicationBuilder
    {
        IApplicationBuilder Use(Func<RequestDelegate, RequestDelegate> component);

        RequestDelegate Build();
    }
}