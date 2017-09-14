using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OICNet.Server.Hosting;
using OICNet.Server.ResourceRepository.Internal;
using System.Linq;

namespace OICNet.Server.ResourceRepository
{
    public class ResourceRepositoryMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ResourceRepositoryOptions _options;
        private IOicResourceRepository _resourceRepository;
        private readonly OicConfiguration _oicConfiguration;

        public ResourceRepositoryMiddleware(RequestDelegate next, IServiceProvider services, IOptions<ResourceRepositoryOptions> options)
        {
            _next = next;
            _options = options.Value;
            _oicConfiguration = services.GetRequiredService<OicConfiguration>();
            _resourceRepository = _options.ResourceRepository
                                ?? ActivatorUtilities.CreateInstance<DefaultResourceRepository>(services);
        }

        public async Task Invoke(OicContext context)
        {
            var resourceContext = new ResourceRepositoryContext(context, _options, _resourceRepository);

            if (!resourceContext.ValidatePath())
            {
                await _next(context);
                return;
            }

            OicResponse result;

            var requestResource = new OicRemoteResourceRepository(_oicConfiguration, context.Request, resourceContext.GetPath().AbsolutePath);

            if (context.Request.Operation == OicRequestOperation.Get)
            {
                result = await _resourceRepository.RetrieveAsync(requestResource);
            }
            else if (context.Request.Operation == OicRequestOperation.Post)
            {
                result = await _resourceRepository.CreateOrUpdateAsync(requestResource);
            }
            else if (context.Request.Operation == OicRequestOperation.Put)
            {
                result = await _resourceRepository.CreateAsync(requestResource);
            }
            else if (context.Request.Operation == OicRequestOperation.Delete)
            {
                result = await _resourceRepository.DeleteAsync(requestResource);
            }
            else
            {
                throw new InvalidOperationException($"TODO: resopnd with a {nameof(OicResponseCode.BadRequest)}");
            }

            context.Response = result;

        }
    }
}