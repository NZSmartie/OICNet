using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OICNet.Server.Hosting;

namespace OICNet.Server.ResourceRepository.Internal
{
    /// <summary>
    /// Finds the first implementation of <see cref="IOicResourceRepository"/> in <see cref="IHostingEnvironment.ApplicationName"/> and invokes <see cref="GetResource"/>
    /// </summary>
    public class DefaultResourceRepository : IOicResourceRepository
    {
        private readonly ILogger<DefaultResourceRepository> _logger;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IOicResourceRepository _resourceRepository;

        public DefaultResourceRepository(IHostingEnvironment hostingEnvironment, ILogger<DefaultResourceRepository> logger, IServiceProvider service)
        {
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;

            // First, check IServiceProvider for any IOicResourceRepository instances
            var repositoryServices = service.GetServices<IOicResourceRepository>();

            var defaultService = repositoryServices.FirstOrDefault(r => r.GetType().GetCustomAttribute<DefaultResourceRepositoryAttribute>() != null);
            if (defaultService != null)
            {
                _resourceRepository = defaultService;
            }
            else if (repositoryServices.Count() == 1)
            {
                _resourceRepository = repositoryServices.First();
            }
            else if (repositoryServices.Count() > 1)
            {
                throw new NotSupportedException($"Multiple {nameof(IOicResourceRepository)} instances were provided in {nameof(IServiceProvider)}. Please add {nameof(DefaultResourceRepositoryAttribute)} to one, or remove multiple instances");
            }
            else
            {
                // TODO: IOicRepositoryContext to return a repository?
                var repositories = Assembly.Load(new AssemblyName(hostingEnvironment.ApplicationName))
                    .ExportedTypes
                    .Where(t => typeof(IOicResourceRepository).IsAssignableFrom(t));

                var repository = GetRepository(repositories);

                if (repository != null)
                    _resourceRepository = (IOicResourceRepository)ActivatorUtilities.CreateInstance(service, repository);
            }
        }

        private Type GetRepository(IEnumerable<Type> repositories)
        {
            var meh = repositories.FirstOrDefault(t => t.GetCustomAttribute<DefaultResourceRepositoryAttribute>() != null);
            if (meh != null)
                return meh;

            using (var e = repositories.GetEnumerator())
            {
                if (e.MoveNext())
                    meh = e.Current;
                if (e.MoveNext())
                    throw new NotSupportedException($"Multiple {nameof(IOicResourceRepository)} instances were found in {_hostingEnvironment.ApplicationName}. Please add {nameof(DefaultResourceRepositoryAttribute)} to one, or remove multiple implemtations");
            }
            return meh;
        }

        public Task<OicResponse> CreateAsync(OicRequest request, IOicResource resource)
        {
            return _resourceRepository.CreateAsync(request, resource);
        }

        public Task<OicResponse> RetrieveAsync(OicRequest request)
        {
            return _resourceRepository.RetrieveAsync(request);
        }

        public Task<OicResponse> CreateOrUpdateAsync(OicRequest request, IOicResource resource)
        {
            return _resourceRepository.CreateOrUpdateAsync(request, resource);
        }

        public Task<OicResponse> DeleteAsync(OicRequest request)
        {
            return _resourceRepository.DeleteAsync(request);
        }
    }
}