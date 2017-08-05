using System.Collections.Generic;
using System.Threading.Tasks;

namespace OICNet
{
    public interface IOicResource
    {
        OicDevice Device { get; }

        string RelativeUri { get; }

        string Id { get; }

        List<OicResourceInterface> Interfaces { get; }

        string Name { get; }

        List<string> ResourceTypes { get; }

        Task CreateAsync(IOicResource resource);

        Task UpdateAsync(IOicResource resource);

        Task DeleteAsync();

        Task RetrieveAsync();
    }
}