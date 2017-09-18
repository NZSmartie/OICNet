using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OICNet
{
    public class OicRemoteResourceRepository : IOicRemoteResourceRepository
    {
        public IList<IOicResource> Resourcse { get; }

        public OicRemoteDevice Device { get; }

        public OicRemoteResourceRepository(OicRemoteDevice device = null)
        {
            Device = device;
        }

        public virtual Task<OicResponse> CreateAsync(OicRequest request, IOicResource resource)
        {
            throw new NotSupportedException();
        }

        public virtual Task<OicResponse> CreateOrUpdateAsync(OicRequest request, IOicResource resource)
        {
            throw new NotSupportedException();
        }

        public virtual Task<OicResponse> DeleteAsync(OicRequest request)
        {
            throw new NotSupportedException();
        }

        public virtual async Task<OicResponse> RetrieveAsync(OicRequest request)
        {
            if (Device == null)
                throw new NullReferenceException($"{GetType().FullName}.{nameof(Device)} cannot be null null");

            var endoint = Device.Endpoint;

            return await endoint.Transport.SendMessageWithResponseAsync(endoint, request);

            //using (var results = Device.Configuration.Serialiser.Deserialise(response.Content, response.ContentType)
            //    .GetEnumerator())
            //{
            //    results.MoveNext();
            //    var result = results.Current;

            //    resource.UpdateFields(result);

            //    // We should not have more than one result in a response to a Retreive.
            //    if (results.MoveNext())
            //        throw new InvalidOperationException($"Received multiple objects during {nameof(RetrieveAsync)}");
            //}

            //return Task.FromResult(response);
        }
    }
}
