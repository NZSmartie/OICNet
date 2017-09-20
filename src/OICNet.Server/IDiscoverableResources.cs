using OICNet.CoreResources;
using System.Collections.Generic;

namespace OICNet.Server
{
    public interface IDiscoverableResources
    {
        IEnumerable<OicResourceLink> DiscoverableResources { get ; }
    }
}
