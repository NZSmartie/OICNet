using System.Collections.Generic;

namespace OICNet
{
    public interface IOicResource
    {
        string RelativeUri { get; }
        string Id { get; }
        List<OicResourceInterface> Interfaces { get; }
        string Name { get; }
        List<string> ResourceTypes { get; }
    }
}