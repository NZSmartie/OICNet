using System;
using Microsoft.Extensions.Options;

using OICNet.Server.ResourceRepository;

// ReSharper disable once CheckNamespace
namespace OICNet.Server.Builder
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

            return app.UseResourceRepository(options =>
            {
                options.RequestPath = requestPath;
            });
        }

        public static IApplicationBuilder UseResourceRepository<TRepository>(this IApplicationBuilder app, string requestPath, params object[] args) where TRepository : class, IOicResourceRepository
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));

            return app.UseResourceRepository(options =>
            {
                options.RequestPath = requestPath;
                options.UseResourceRepository<TRepository>(args);
            });
        }

        public static IApplicationBuilder UseResourceRepository(this IApplicationBuilder app, Action<ResourceRepositoryOptions> options)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            var resourceRepositoryOptions = new ResourceRepositoryOptions();
            options(resourceRepositoryOptions);
            return app.UseMiddleware<ResourceRepositoryMiddleware>(Options.Create(resourceRepositoryOptions));
        }
    }
}
