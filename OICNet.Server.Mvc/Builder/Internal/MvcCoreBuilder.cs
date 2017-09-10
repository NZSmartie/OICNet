using System;
using Microsoft.Extensions.DependencyInjection;
using OICNet.Server.Mvc.ApplicationParts;

namespace OICNet.Server.Mvc.Builder.Internal
{
    public class MvcCoreBuilder : IMvcCoreBuilder
    {
        public IServiceCollection Services { get; }
        public ApplicationPartManager PartManager { get; }

        public MvcCoreBuilder(IServiceCollection services, ApplicationPartManager partManager)
        {
            Services = services;
            PartManager = partManager;
        }
    }
}