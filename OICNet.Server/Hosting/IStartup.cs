using System;
using Microsoft.Extensions.DependencyInjection;
using OICNet.Server.Builder;

namespace OICNet.Server.Hosting
{
    public interface IStartup
    {
        void Configure(IApplicationBuilder app);

        IServiceProvider ConfigureServices(IServiceCollection services);
    }
}