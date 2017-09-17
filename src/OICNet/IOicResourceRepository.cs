using System.Threading.Tasks;

namespace OICNet
{
    public interface IOicResourceRepository
    {
        Task<OicResponse> CreateAsync(string path, IOicResource resource);

        Task<OicResponse> CreateOrUpdateAsync(string path, IOicResource resource);

        Task<OicResponse> DeleteAsync(string path);

        Task<OicResponse> RetrieveAsync(string path);
    }

    public interface IOicRemoteResourceRepository : IOicResourceRepository
    {
        OicRemoteDevice Device { get; }
    }

}
