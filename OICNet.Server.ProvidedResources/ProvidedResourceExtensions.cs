using System;
using Microsoft.Extensions.Options;
using OICNet.Server.Builder;
using OICNet.Server.ProvidedResources;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class ProvidedResourceExtensions
    {
        public static IApplicationBuilder UseProvidedResources(this IApplicationBuilder app)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));

            return app.UseMiddleware<ProvidedResourceMiddleware>();
        }

        public static IApplicationBuilder UseProvidedResources(this IApplicationBuilder app, string requestPath)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));

            return app.UseProvidedResources(new ProvidedResourceOptions
            {
                RequestPath = requestPath
            });
        }

        public static IApplicationBuilder UseProvidedResources(this IApplicationBuilder app, ProvidedResourceOptions options)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            return app.UseMiddleware<ProvidedResourceMiddleware>(Options.Options.Create(options));
        }
    }
}
