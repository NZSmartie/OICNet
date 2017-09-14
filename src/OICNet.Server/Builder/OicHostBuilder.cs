using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using OICNet.Server.Builder.Internal;
using OICNet.Server.Hosting;
using OICNet.Server.Hosting.Internal;

namespace OICNet.Server.Builder
{
    public class OicHostBuilder
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IList<Action<OicHostBuilderContext, IServiceCollection>> _configureServicesDelegates = new List<Action<OicHostBuilderContext, IServiceCollection>>();

        private readonly IConfiguration _config;

        private readonly OicHostBuilderContext _context;
        private OicHostOptions _options;

        public OicHostBuilder()
        {
            _hostingEnvironment = new HostingEnvironment();
            _config = new ConfigurationBuilder()
                .AddEnvironmentVariables(prefix: "OICNET_")
                .Build();

            _context = new OicHostBuilderContext
            {
                Configuration = _config
            };
        }

        public OicHostBuilder UseSetting(string key, string value)
        {
            _config[key] = value;
            return this;
        }

        public string GetSetting(string key)
        {
            return _config[key];
        }

        public OicHostBuilder UseConfiguration(IConfiguration config)
        {
            foreach (var item in config.AsEnumerable())
                _config[item.Key] = item.Value;
            return this;
        }

        /// <summary>
        /// Adds a delegate for configuring additional services for the host or web application. This may be called
        /// multiple times.
        /// </summary>
        /// <param name="configureServices">A delegate for configuring the <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="OicHostBuilder"/>.</returns>
        public OicHostBuilder ConfigureServices(Action<IServiceCollection> configureServices)
        {
            if (configureServices == null)
            {
                throw new ArgumentNullException(nameof(configureServices));
            }

            return ConfigureServices((_, services) => configureServices(services));
        }

        /// <summary>
        /// Adds a delegate for configuring additional services for the host or web application. This may be called
        /// multiple times.
        /// </summary>
        /// <param name="configureServices">A delegate for configuring the <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="OicHostBuilder"/>.</returns>
        public OicHostBuilder ConfigureServices(Action<OicHostBuilderContext, IServiceCollection> configureServices)
        {
            if (configureServices == null)
            {
                throw new ArgumentNullException(nameof(configureServices));
            }

            _configureServicesDelegates.Add(configureServices);
            return this;
        }

        public OicHost Build()
        {
            var hostingServices = BuildCommonServices();

            // Create a IServiceCollection for the applciation container based on the host's application container. 
            var applicationServiceCollection = new ServiceCollection();
            foreach (var service in hostingServices)
                applicationServiceCollection.Add(service);

            return new OicHost(applicationServiceCollection, hostingServices.BuildServiceProvider(), _config);
        }

        private IServiceCollection BuildCommonServices()
        {
            // TODO: find all IHostingStartup implementations and configure this OicHostBuilder?
            _options = new OicHostOptions(_config);

            _hostingEnvironment.Initialize(_options);

            var services = new ServiceCollection();

            services.AddSingleton(_hostingEnvironment);
            services.AddLogging();

            services.AddScoped<IMiddlewareFactory, DefaultMiddlewareFactory>();
            services.AddTransient<IServiceProviderFactory<IServiceCollection>, DefaultServiceProviderFactory>();

            // Conjure up a RequestServices
            services.AddTransient<IStartupFilter, AutoRequestServicesStartupFilter>();

            foreach (var configureServiceDelegate in _configureServicesDelegates)
                configureServiceDelegate(_context, services);

            if (!services.Any(sd => typeof(OicConfiguration).IsAssignableFrom(sd.ImplementationType)))
                services.AddSingleton(OicConfiguration.Default);

            services.AddOptions();

            return services;
        }
    }
}