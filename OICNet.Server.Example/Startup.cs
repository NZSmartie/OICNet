using System;
using Microsoft.Extensions.DependencyInjection;
using OICNet.Server.Builder;
using OICNet.Server.Hosting;

namespace OICNet.Server.Example
{
    public class Startup : IStartup
    {
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            return services
                //.AddMvc()
                .BuildServiceProvider();
        }

        public void Configure(IApplicationBuilder app)
        {
            //app.UseMvc();
        }
    }
}