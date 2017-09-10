using Microsoft.Extensions.DependencyInjection;
using OICNet.Server.Mvc.ApplicationParts;

namespace OICNet.Server.Mvc.Builder
{
    public interface IMvcCoreBuilder
    {
        IServiceCollection Services { get; }
        ApplicationPartManager PartManager { get; }
    }
}