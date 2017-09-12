using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OICNet.Server.Builder;
using OICNet.Server.Hosting;
using OICNet.Server.ProvidedResources;

namespace OICNet.Server.Example
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Optional: Provide our OIC configuration here, or use the detault. if omitted, the default will be used.
            services.AddSingleton(OicConfiguration.Default);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseProvidedResources("test");
        }
    }
}