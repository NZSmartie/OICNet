using System.Threading.Tasks;

namespace OICNet
{
    public interface IOicResourceRepository
    {
        Task<OicResponse> CreateAsync(IOicResource resource);

        Task<OicResponse> CreateOrUpdateAsync(IOicResource resource);

        Task<OicResponse> DeleteAsync(IOicResource resource);

        Task<OicResponse> RetrieveAsync(IOicResource resource);
    }

    public interface IOicRemoteResourceRepository : IOicResourceRepository
    {
        OicDevice Device { get; }
    }

}
