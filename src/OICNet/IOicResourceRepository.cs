using System.Threading.Tasks;

namespace OICNet
{
    public interface IOicResourceRepository
    {
        Task<OicResponse> CreateAsync(OicRequest request, IOicResource resource);

        Task<OicResponse> CreateOrUpdateAsync(OicRequest request, IOicResource resource);

        Task<OicResponse> DeleteAsync(OicRequest request);

        Task<OicResponse> RetrieveAsync(OicRequest request);
    }

    public interface IOicRemoteResourceRepository : IOicResourceRepository
    {
        OicRemoteDevice Device { get; }
    }

}
