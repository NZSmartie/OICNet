using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OICNet.Server.Hosting;
using OICNet.Server.Hosting.Internal;

namespace OICNet.Server.Builder
{
    public static class OicHostBuilderExtensions
    {
        /// <summary>
        /// Specify the startup method to be used to configure the web application.
        /// </summary>
        /// <param name="hostBuilder">The <see cref="IWebHostBuilder"/> to configure.</param>
        /// <param name="configureApp">The delegate that configures the <see cref="IApplicationBuilder"/>.</param>
        /// <returns>The <see cref="IWebHostBuilder"/>.</returns>
        public static OicHostBuilder Configure(this OicHostBuilder hostBuilder, Action<IApplicationBuilder> configureApp)
        {
            if (configureApp == null)
            {
                throw new ArgumentNullException(nameof(configureApp));
            }

            var startupAssemblyName = configureApp.GetMethodInfo().DeclaringType.GetTypeInfo().Assembly.GetName().Name;

            return hostBuilder
                .UseSetting(OicHostDefaults.ApplicationKey, startupAssemblyName)
                .ConfigureServices(services =>
                {
                    services.AddSingleton<IStartup>(sp =>
                    {
                        return new DelegateStartup(sp.GetRequiredService<IServiceProviderFactory<IServiceCollection>>(), configureApp);
                    });
                });
        }

        public static OicHostBuilder ConfigureLogging(this OicHostBuilder builder, Action<ILoggingBuilder> configureLogging)
        {
            return builder.ConfigureServices(collection => collection.AddLogging(configureLogging));
        }

        /// <summary>
        /// Specify the startup type to be used by the web host.
        /// </summary>
        /// <param name="hostBuilder">The <see cref="OicHostBuilder"/> to configure.</param>
        /// <param name="startupType">The <see cref="Type"/> to be used.</param>
        /// <returns>The <see cref="OicHostBuilder"/>.</returns>
        public static OicHostBuilder UseStartup(this OicHostBuilder hostBuilder, Type startupType)
        {
            var startupAssemblyName = startupType.GetTypeInfo().Assembly.GetName().Name;

            return hostBuilder
                .UseSetting(OicHostDefaults.ApplicationKey, startupAssemblyName)
                .ConfigureServices(services =>
                {
                    if (typeof(IStartup).GetTypeInfo().IsAssignableFrom(startupType.GetTypeInfo()))
                    {
                        services.AddSingleton(typeof(IStartup), startupType);
                    }
                    else
                    {
                        services.AddSingleton(typeof(IStartup), sp =>
                        {
                            var hostingEnvironment = sp.GetRequiredService<IHostingEnvironment>();
                            return new ConventionBasedStartup(StartupLoader.LoadMethods(sp, startupType, hostingEnvironment.EnvironmentName));
                        });
                    }
                });
        }

        /// <summary>
        /// Specify the startup type to be used by the web host.
        /// </summary>
        /// <param name="hostBuilder">The <see cref="OicHostBuilder"/> to configure.</param>
        /// <typeparam name ="TStartup">The type containing the startup methods for the application.</typeparam>
        /// <returns>The <see cref="OicHostBuilder"/>.</returns>
        public static OicHostBuilder UseStartup<TStartup>(this OicHostBuilder hostBuilder) where TStartup : class
        {
            return hostBuilder.UseStartup(typeof(TStartup));
        }
    }
}