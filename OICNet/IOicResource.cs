using System.Collections.Generic;
using System.Threading.Tasks;

namespace OICNet
{
    public interface IOicResource
    {
        OicDevice Device { get; set; }

        string RelativeUri { get; set; }

        string Id { get; set; }

        List<OicResourceInterface> Interfaces { get; set; }

        string Name { get; set; }

        List<string> ResourceTypes { get; set; }

        Task CreateAsync(IOicResource resource);

        Task UpdateAsync(IOicResource resource);

        Task DeleteAsync();

        Task RetrieveAsync();
    }
}