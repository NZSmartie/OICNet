using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OICNet.Server.Hosting;
using OICNet.Server.ResourceRepository.Internal;
using System.Linq;
using OICNet.Utilities;

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
                                ?? (_options.ResourceRepositoryType != null
                                    ? (IOicResourceRepository)ActivatorUtilities.CreateInstance(services, _options.ResourceRepositoryType, _options.ResourceRepositoryArgs)
                                    : ActivatorUtilities.CreateInstance<DefaultResourceRepository>(services));
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

            var path = resourceContext.GetPath().AbsolutePath;

            IOicResource requestResource = null;
            if (context.Request.ContentType != OicMessageContentType.None) {
                if (context.Request.Content == null)
                {
                    context.Response = OicResponseUtility.CreateMessage(OicResponseCode.BadRequest, "Content type provided with no content");
                    return;
                }

                // TODO: verify grabbing the first resource is okay and enumeration is not needed.
                requestResource = _oicConfiguration.Serialiser.Deserialise(context.Request.Content, context.Request.ContentType).First();
            }

            if (context.Request.Operation == OicRequestOperation.Get)
            {
                result = await _resourceRepository.RetrieveAsync(path);
            }
            else if (context.Request.Operation == OicRequestOperation.Post)
            {
                result = await _resourceRepository.CreateOrUpdateAsync(path, requestResource);
            }
            else if (context.Request.Operation == OicRequestOperation.Put)
            {
                result = await _resourceRepository.CreateAsync(path, requestResource);
            }
            else if (context.Request.Operation == OicRequestOperation.Delete)
            {
                result = await _resourceRepository.DeleteAsync(path);
            }
            else
            {
                result = OicResponseUtility.CreateMessage(OicResponseCode.BadRequest, "Bad request");
            }

            context.Response = result;

        }
    }
}