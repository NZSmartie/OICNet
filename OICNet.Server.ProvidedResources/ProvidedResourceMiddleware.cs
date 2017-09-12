using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OICNet.Server.Hosting;
using OICNet.Server.ProvidedResources.Internal;

namespace OICNet.Server.ProvidedResources
{
    public class ProvidedResourceMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ProvidedResourceOptions _options;
        private IOicResourceProvider _resourceProvider;
        private readonly OicConfiguration _oicConfiguration;

        public ProvidedResourceMiddleware(RequestDelegate next, IServiceProvider services, IOptions<ProvidedResourceOptions> options)
        {
            _next = next;
            _options = options.Value;
            _oicConfiguration = services.GetRequiredService<OicConfiguration>();
            _resourceProvider = _options.ResourceProvider
                                ?? ActivatorUtilities.CreateInstance<DefaultResourceProvider>(services);
        }

        public Task Invoke(OicContext context)
        {
            var resourceContext = new OicResourceContext(context, _options, _resourceProvider);

            if (!resourceContext.ValidatePath())
                return _next(context);

            var resource = _resourceProvider.GetResource(resourceContext.GetPath());
            if(resource == null)
                return _next(context);

            context.Response = new OicResourceResponse(_oicConfiguration, resource)
            {
                ResposeCode = OicResponseCode.Content,
                ContentType = OicMessageContentType.ApplicationCbor
            };

            return Task.CompletedTask;
        }
    }
}