using Microsoft.Extensions.DependencyInjection;
using OICNet.Server.Builder;

namespace OICNet.Server.Mvc
{
    public static class Extensions
    {
        public static IApplicationBuilder UseMvc(this IApplicationBuilder builder)
        {
            return builder;
        }

        public static IServiceCollection AddMvc(this IServiceCollection services)
        {
            return services;
        }
    }
}