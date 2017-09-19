using Microsoft.Extensions.DependencyInjection;
using OICNet.Server.Builder;
using OICNet.Server.Example.Devices;

namespace OICNet.Server.Example
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                // Optional: Provide our OIC configuration here, or use the detault. if omitted, the default will be used.
                .AddSingleton(new OicConfiguration(new MyResourceResolver()))
                .AddSingleton<IOicResourceRepository, MyResources>()
                .AddOicResources();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseResourceRepository("test");

            // Provide default /oic/ resources for our LightDevice
            app.UseOicResources<LightDevice>();
        }
    }
}