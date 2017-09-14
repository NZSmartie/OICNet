using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using CoAPNet;
using CoAPNet.Server;
using CoAPNet.Utils;

using OICNet.Server.CoAP.Utils;
using OICNet.Server.Hosting;
using OICNet.Utilities;

namespace OICNet.Server.CoAP.Internal
{
    internal class OicCoapHandler : CoapHandler
    {
        private readonly OicHostApplication _application;
        private readonly ILogger<OicCoapHandler> _logger;

        public OicCoapHandler(OicHostApplication application, ILogger<OicCoapHandler> logger)
        {
            _logger = logger;
            _application = application;
        }

        protected override async Task<CoapMessage> HandleRequestAsync(ICoapConnectionInformation connectionInformation, CoapMessage message)
        {
            _logger.LogDebug($"New request! {message}");

            var response = OicResponseUtility.CreateMessage(OicResponseCode.NotFound, "Not found");
            OicContext context = null;
            Exception exception = null;

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
    }
}