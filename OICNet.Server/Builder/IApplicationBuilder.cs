using System;
using OICNet.Server.Hosting;

namespace OICNet.Server.Builder
{
    public interface IApplicationBuilder
    {
        IServiceProvider ApplicationServices { get; }

        IApplicationBuilder Use(Func<RequestDelegate, RequestDelegate> component);

        RequestDelegate Build();
    }
}