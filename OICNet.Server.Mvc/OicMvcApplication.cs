using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OICNet.Server.Hosting;
using OICNet.Server.Mvc;

namespace OICNet.Server
{
    public class OicMvcApplication
    {
        private readonly RequestDelegate _next;
        private IServiceProvider _serviceProvider;

        public OicMvcApplication(RequestDelegate next, IServiceProvider serviceProvider)
        {
            _next = next;
            _serviceProvider = serviceProvider;
        }

        public RequestDelegate RequestDelegate => Invoke;

        public Task Invoke(OicContext context)
        {
            var controllers = _serviceProvider.GetServices<OicController>();

            return _next.Invoke(context);
        }
    }
}