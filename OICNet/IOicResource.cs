using System.Collections.Generic;
using System.Threading.Tasks;

namespace OICNet
{
    public interface IOicResource
    {
        OicDevice Device { get; set; }

        string RelativeUri { get; set; }

        string Id { get; set; }

        IList<OicResourceInterface> Interfaces { get; }

        string Name { get; set; }

        IList<string> ResourceTypes { get; }

        Task CreateAsync(IOicResource resource);

        Task UpdateAsync(IOicResource resource);

        Task DeleteAsync();

        Task RetrieveAsync();

        void UpdateFields(IOicResource source);
    }
}