using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using OICNet.Server.Hosting;
using OICNet.Server.Hosting.Internal;

namespace OICNet.Server.Builder
{
    public class OicHostBuilder
    {
        private readonly IList<Action<IServiceCollection>> _configureServicesDelegates = new List<Action<IServiceCollection>>();
        private readonly IList<Action<ILoggerFactory>> _configureLoggingDelegates = new List<Action<ILoggerFactory>>();
        private readonly IConfiguration _config;
        private readonly IHostingEnvironment _hostingEnvironment;
        private ILoggerFactory _loggerFactory;

        public OicHostBuilder()
        {
            _hostingEnvironment = new HostingEnvironment();
            _config = new ConfigurationBuilder()
                .AddEnvironmentVariables(prefix: "OICNET_")
                .Build();
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

        public OicHostBuilder UseStartup<TStartup>() where TStartup : class, IStartup
        {
            // TODO: Support Startup by convention rather by implementing IStartup
            return ConfigureServices(services => services.AddTransient<IStartup, TStartup>());
        }

        public OicHostBuilder ConfigureServices(Action<IServiceCollection> configureServices)
        {
            _configureServicesDelegates.Add(configureServices ?? throw new ArgumentNullException(nameof(configureServices)));
            return this;
        }

        public OicHostBuilder ConfigureLogging(Action<ILoggerFactory> configureLogging)
        {
            _configureLoggingDelegates.Add(configureLogging ?? throw new ArgumentNullException(nameof(configureLogging)));
            return this;
        }

        public OicHost Build()
        {
            var services = new ServiceCollection();

            services.AddSingleton(_hostingEnvironment);

            if(_loggerFactory == null)
                _loggerFactory = new LoggerFactory();

            
            foreach (var configureLoggingDelegate in _configureLoggingDelegates)
                configureLoggingDelegate(_loggerFactory);

            services.AddSingleton(_loggerFactory);
            services.AddLogging();

            foreach (var configureServiceDelegate in _configureServicesDelegates)
                configureServiceDelegate(services);

            services.AddOptions();

            // Create a IServiceCollection for the applciation container based on the host's application container. 
            var applicationServiceCollection = new ServiceCollection();
            foreach (var service in services)
                applicationServiceCollection.Add(service);

            applicationServiceCollection.Replace(ServiceDescriptor.Singleton(_loggerFactory));

            return new OicHost(applicationServiceCollection, services.BuildServiceProvider(), _config);
        }
    }
}