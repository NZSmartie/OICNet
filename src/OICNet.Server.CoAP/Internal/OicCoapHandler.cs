using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using CoAPNet;
using CoAPNet.Server;
using CoAPNet.Utils;
using CoapOptions = CoAPNet.Options;

using OICNet.Server.CoAP.Utils;
using OICNet.Server.Hosting;
using OICNet.Utilities;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Text;

namespace OICNet.Server.CoAP.Internal
{
    internal class OicCoapHandler : CoapHandler
    {
        private readonly OicHostApplication _application;
        private readonly ILogger<OicCoapHandler> _logger;
        private readonly OicCoapServerOptions _options;
        private readonly IDiscoverableResources _discoverableResources;

        public OicCoapHandler(OicHostApplication application, ILogger<OicCoapHandler> logger, OicCoapServerOptions options)
        {
            _logger = logger;
            _options = options;
            _application = application;

            if (options.UseCoreLink)
                _discoverableResources = application.Services.GetRequiredService<IDiscoverableResources>();
        }

        protected override async Task<CoapMessage> HandleRequestAsync(ICoapConnectionInformation connectionInformation, CoapMessage message)
        {
            _logger.LogDebug($"New request! {message}");

            var response = OicResponseUtility.CreateMessage(OicResponseCode.NotFound, "Not found");
            OicContext context = null;
            Exception exception = null;

            if (_options.UseCoreLink && message.GetUri().AbsolutePath.Equals("/.well-known/core", StringComparison.Ordinal))
                return GenerateCoreLinkResource(message);

            try
            {
                // Create the OiCContext
                context = _application.CreateContext(new OicCoapContext(connectionInformation, message));

                await _application.ProcessRequestAsync(context);

                if (context.Response.ResposeCode != default(OicResponseCode))
                {
                    response = context.Response;
                }
                else
                {
                    // This should only occur during development.
                    var errorMessage = $"{context.GetType()}.{nameof(context.Response)}.{nameof(context.Response.ResposeCode)} was not set!";
                    _logger.LogError(errorMessage);
                    response = OicResponseUtility
                        .CreateMessage(OicResponseCode.InternalServerError, errorMessage);
                }
            }
            catch (Exception ex)
            {
                exception = ex;
                // TODO: Do we want to restrict error messages to developers/logging only? 
                response = OicResponseUtility.FromException(ex);
            }
            finally
            {
                _application.DisposeContext<OicCoapContext>(context, exception);
            }

            _logger.LogDebug($"Responding with {response}");
            return response.ToCoapMessage();
        }

        private CoapMessage GenerateCoreLinkResource(CoapMessage message)
        {
            if (message.Code != CoapMessageCode.Get)
                return CoapMessageUtility.FromException(new CoapException("Method not allowed", CoapMessageCode.MethodNotAllowed));

            var links = _discoverableResources.DiscoverableResources
                .Select(r => new CoapResourceMetadata(r.Href)
                {
                    ResourceTypes = r.ResourceTypes,
                    SuggestedContentTypes = { CoapOptions.ContentFormatType.ApplicationCbor, CoapOptions.ContentFormatType.ApplicationJson },
                    Title = r.Title,
                });
            return new CoapMessage
            {
                Code = CoapMessageCode.Content,
                Type = message.Type == CoapMessageType.Confirmable ? CoapMessageType.Acknowledgement : CoapMessageType.NonConfirmable,
                Payload = Encoding.UTF8.GetBytes(CoreLinkFormat.ToCoreLinkFormat(links)),
                Options =
                    {
                        new CoapOptions.ContentFormat(CoapOptions.ContentFormatType.ApplicationLinkFormat)
                    }
            };
        }
    }
}