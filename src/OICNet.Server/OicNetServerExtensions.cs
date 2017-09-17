using System;
using Microsoft.Extensions.DependencyInjection;

using OICNet.Server.Internal;

// ReSharper disable once CheckNamespace
namespace OICNet.Server.Builder
{
    public static class OicNetServerExtensions
    {
        public static IApplicationBuilder UseOicResources(this IApplicationBuilder app)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));

            return app.UseResourceRepository(options =>
            {
                options.RequestPath = "/oic";
                options.UseResourceRepository<OicHostDevice>();
            });
        }

        public static IApplicationBuilder UseOicResources<TOicDevice>(this IApplicationBuilder app) where TOicDevice : OicDevice
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));

            var device = ActivatorUtilities.CreateInstance<TOicDevice>(app.ApplicationServices);

            return app.UseResourceRepository(options =>
            {
                options.RequestPath = "/oic";
                options.UseResourceRepository<OicHostDevice>(device);
            });
        }

        public static IApplicationBuilder UseOicResources(this IApplicationBuilder app, OicDevice device)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));

            return app.UseResourceRepository(options =>
            {
                options.RequestPath = "/oic";
                options.UseResourceRepository<OicHostDevice>(device);
            });
        }
    }
}
