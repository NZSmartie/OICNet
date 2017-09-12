using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OICNet.Server.Hosting;

namespace OICNet.Server.ProvidedResources.Internal
{
    public class DefaultResourceProvider : IOicResourceProvider
    {
        private readonly ILogger<DefaultResourceProvider> _logger;
        private readonly IList<IOicResourceProvider> _assemblyResourceProviders = new List<IOicResourceProvider>();

        public DefaultResourceProvider(IHostingEnvironment hostingEnvironment, ILogger<DefaultResourceProvider> logger, IServiceProvider service)
        {
            _logger = logger;

            var assembly = Assembly.Load(new AssemblyName(hostingEnvironment.ApplicationName));
            _assemblyResourceProviders = assembly.ExportedTypes
                .Where(t => typeof(IOicResourceProvider).IsAssignableFrom(t))
                .Select(t => (IOicResourceProvider) ActivatorUtilities.CreateInstance(service, t))
                .ToList();
        }

        public IOicResource GetResource(string id)
        {
            foreach (var provider in _assemblyResourceProviders)
            {
                var resource = provider.GetResource(id);
                if (resource != null)
                    return resource;
            }
            return null;
        }
    }
}