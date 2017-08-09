using System.Collections.Generic;
using System.Threading.Tasks;

namespace OICNet
{
    public interface IOicResource
    {
        OicDevice Device { get; set; }

        string RelativeUri { get; set; }

        string Id { get; set; }

        List<OicResourceInterface> Interfaces { get; }

        string Name { get; set; }

        List<string> ResourceTypes { get; }

        Task CreateAsync(IOicResource resource);

        Task UpdateAsync(IOicResource resource);

        Task DeleteAsync();

        Task RetrieveAsync();
    }
}