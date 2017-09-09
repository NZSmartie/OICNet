using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OICNet.Server.Hosting;

namespace OICNet.Server.Builder.Internal
{
    public class OicApplicationBuilder : IApplicationBuilder
    {
        private readonly IServiceProvider _applicationServices;
        private readonly IList<Func<RequestDelegate, RequestDelegate>> _applicationLayers = new List<Func<RequestDelegate, RequestDelegate>>();

        public OicApplicationBuilder(IServiceProvider applicationServices)
        {
            _applicationServices = applicationServices;
        }

        public IApplicationBuilder Use(Func<RequestDelegate, RequestDelegate> component)
        {
            _applicationLayers.Add(component ?? throw new ArgumentNullException(nameof(component)));
            return this;
        }

        public RequestDelegate Build()
        {
            RequestDelegate application = context =>
            {
                context.Response.ResposeCode = OicResponseCode.NotFound;
                return Task.CompletedTask;
            };

            foreach (var component in _applicationLayers.Reverse())
                application = component(application);

            return application;
        }
    }
}