using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace OICNet.Server
{
    //Todo implement a MVC pattern based on ASPNet's MVC, to allow plugging into existing solutions or what not
    public class OicServer
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly OicConfiguration _configuration;

        public StringComparison UriPathComparison = StringComparison.OrdinalIgnoreCase;

        public OicServer(IServiceProvider serviceProvider)
        {
            _configuration = _serviceProvider.GetService<OicConfiguration>();
            _serviceProvider = serviceProvider;
        }
    }
}