using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OICNet.Server.Builder.Internal;
using OICNet.Server.Hosting.Internal;

namespace OICNet.Server.Hosting
{
    public class OicHost : IDisposable
    {
        private readonly IServiceCollection _applicationServiceCollection;
        private readonly IServiceProvider _hostingServiceProvider;
        private readonly IConfiguration _config;

        private IOicServer _server;

        private RequestDelegate _application;
        private ILogger<OicHost> _logger;
        private IStartup _startup;
        private IServiceProvider _applicationServices;
        private readonly ApplicationLifetime _applicationLifetime;
        
        public IServiceProvider Services => _applicationServices;

        public OicHost(IServiceCollection applicationServiceCollection, IServiceProvider hostingServiceProvider,
            IConfiguration config)
        {
            _applicationServiceCollection = applicationServiceCollection;
            _hostingServiceProvider = hostingServiceProvider ?? throw new ArgumentNullException(nameof(hostingServiceProvider));
            _config = config ?? throw new ArgumentNullException(nameof(config));

            _applicationLifetime = new ApplicationLifetime();
            _applicationServiceCollection.AddSingleton<IApplicationLifetime>(_applicationLifetime);
        }

        public virtual void Start()
        {
            if (_application == null)
                _application = BuildApplication();

            _logger = _hostingServiceProvider.GetRequiredService<ILogger<OicHost>>();

            _server = _hostingServiceProvider.GetRequiredService<IOicServer>();

            var hostApplication = ActivatorUtilities.CreateInstance<OicHostApplication>(_hostingServiceProvider, _application);

            _server.Start(hostApplication);
            _applicationLifetime.NotifyStarted();
        }


        public RequestDelegate BuildApplication()
        {
            _startup = _hostingServiceProvider.GetRequiredService<IStartup>();
            _applicationServices = _startup.ConfigureServices(_applicationServiceCollection);

            var builder = new OicApplicationBuilder(_applicationServices);
            _startup.Configure(builder);

            return builder.Build();
        }

        public void Dispose()
        {
            (_hostingServiceProvider as IDisposable)?.Dispose();
            _applicationLifetime.NotifyStopped();
        }
    }
}