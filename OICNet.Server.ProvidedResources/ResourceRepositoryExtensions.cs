using System;
using OICNet.Server.Builder;
using OICNet.Server.ResourceRepository;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class ResourceRepositoryExtensions
    {
        public static IApplicationBuilder UseResourceRepository(this IApplicationBuilder app)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));

            return app.UseMiddleware<ResourceRepositoryMiddleware>();
        }

        public static IApplicationBuilder UseResourceRepository(this IApplicationBuilder app, string requestPath)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));

            return app.UseResourceRepository(new ResourceRepositoryOptions
            {
                RequestPath = requestPath
            });
        }

        public static IApplicationBuilder UseResourceRepository(this IApplicationBuilder app, ResourceRepositoryOptions options)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            return app.UseMiddleware<ResourceRepositoryMiddleware>(Options.Options.Create(options));
        }
    }
}
