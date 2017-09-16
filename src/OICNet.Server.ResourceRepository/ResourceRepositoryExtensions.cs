using System;
using OICNet.Server.Builder;
using OICNet.Server.ResourceRepository;
using OICNet;

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
            return app.UseMiddleware<ResourceRepositoryMiddleware>(Options.Options.Create(resourceRepositoryOptions));
        }
    }
}
