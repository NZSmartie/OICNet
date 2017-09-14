using System.Threading.Tasks;
using CoAPNet;
using CoAPNet.Server;
using Microsoft.Extensions.Logging;
using OICNet.Server.CoAP.Utils;
using OICNet.Server.Hosting;
using CoAPNet.Utils;
using System;
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

            var context = _application.CreateContext(new OicCoapContext(connectionInformation, message));
            Exception exception = null;

            try
            {
                await _application.ProcessRequestAsync(context);
            }
            catch(Exception ex)
            {
                exception = ex;
                // TODO: Do we want to restrict error messages to developers/logging only? 
                context.Response = OicResponseUtility.FromException(ex);
            }
            
            _logger.LogDebug($"Responding with {context.Response}");

            var response = context.Response.ResposeCode != default(OicResponseCode)
                ? context.Response.ToCoapMessage()
                : null;

            if (response == null)
            {
                // This should only occur during development.
                var errorMessage = $"{context.GetType()}.{nameof(context.Response)}.{nameof(context.Response.ResposeCode)} was not set!";
                _logger.LogError(errorMessage);
                response = OicResponseUtility
                    .CreateMessage(OicResponseCode.InternalServerError, errorMessage)
                    .ToCoapMessage();
            }

            _application.DisposeContext<OicCoapContext>(context, exception);

            return response;
        }
    }
}