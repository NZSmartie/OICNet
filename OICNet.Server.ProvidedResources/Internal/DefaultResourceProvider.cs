using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OICNet.Server.Hosting;

namespace OICNet.Server.ProvidedResources.Internal
{
    /// <summary>
    /// Finds the first implementation of <see cref="IOicResourceProvider"/> in <see cref="IHostingEnvironment.ApplicationName"/> and invokes <see cref="GetResource"/>
    /// </summary>
    public class DefaultResourceProvider : IOicResourceProvider
    {
        private readonly ILogger<DefaultResourceProvider> _logger;
        private readonly IOicResourceProvider _assemblyResourceProvider;

        public DefaultResourceProvider(IHostingEnvironment hostingEnvironment, ILogger<DefaultResourceProvider> logger, IServiceProvider service)
        {
            _logger = logger;

            var assembly = Assembly.Load(new AssemblyName(hostingEnvironment.ApplicationName));
            var providers = assembly.ExportedTypes
                .Where(t => typeof(IOicResourceProvider).IsAssignableFrom(t))
                .Select(t => (IOicResourceProvider)ActivatorUtilities.CreateInstance(service, t)).GetEnumerator();

            if (providers.MoveNext())
            {
                _assemblyResourceProvider = providers.Current;
                if (providers.MoveNext())
                    throw new NotSupportedException($"Multiple {nameof(IOicResourceProvider)} instances were found in {hostingEnvironment.ApplicationName}");
            }
        }

        public IList<IOicResource> Resources => throw new NotSupportedException();

        public IOicResource GetResource(Uri uri) => _assemblyResourceProvider?.GetResource(uri);
    }
}