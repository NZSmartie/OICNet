using System.Threading.Tasks;
using CoAPNet;
using CoAPNet.Server;
using Microsoft.Extensions.Logging;
using OICNet.Server.CoAP.Utils;
using OICNet.Server.Hosting;

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

            await _application.ProcessRequestAsync(context);

            _logger.LogDebug($"Responding with {context.Response}");

            var response = context.Response.ResposeCode != default(OicResponseCode)
                ? context.Response.ToCoapMessage()
                : null;

            _application.DisposeContext<OicCoapContext>(context, null);

            return response;
        }
    }
}