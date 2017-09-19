using System;
using Microsoft.Extensions.DependencyInjection;

using OICNet.Server.Internal;

// ReSharper disable once CheckNamespace
namespace OICNet.Server.Builder
{
    public static class OicNetServerExtensions
    {
        public static IServiceCollection AddOicResources(this IServiceCollection services)
        {
            return services.AddSingleton<OicHostDevice>()
                           .AddSingleton<IDiscoverableResources>(s => s.GetService<OicHostDevice>());
        }

        public static IApplicationBuilder UseOicResources(this IApplicationBuilder app)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));

            var host = app.ApplicationServices.GetService<OicHostDevice>();

            return app.UseResourceRepository(options =>
            {
                options.ResourceRepository = host;
            });
        }

        public static IApplicationBuilder UseOicResources<TOicDevice>(this IApplicationBuilder app) where TOicDevice : OicDevice
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));

            var host = app.ApplicationServices.GetService<OicHostDevice>();
            host.AddDevice(ActivatorUtilities.CreateInstance<TOicDevice>(app.ApplicationServices));

            return app.UseResourceRepository(options =>
            {
                options.ResourceRepository = host;
            });
        }

        public static IApplicationBuilder UseOicResources(this IApplicationBuilder app, OicDevice device)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));

            var host = app.ApplicationServices.GetService<OicHostDevice>();
            host.AddDevice(device);

            return app.UseResourceRepository(options =>
            {
                options.ResourceRepository = host;
            });
        }
    }
}
