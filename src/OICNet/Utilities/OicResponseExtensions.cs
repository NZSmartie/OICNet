using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OICNet.Utilities
{
    public static class OicResponseExtensions
    {
        public static IEnumerable<IOicResource> GetResources(this OicResponse response, OicConfiguration configuration)
        {
            if (response is OicResourceResponse r)
                return new[] { r.Resource }.AsEnumerable();

            if (response.ContentType == OicMessageContentType.None)
                return Enumerable.Empty<IOicResource>();

            return configuration.Serialiser.Deserialise(response.Content, response.ContentType);
        }

        public static IOicResource GetResource(this OicResponse response, OicConfiguration configuration)
        {
            if (response is OicResourceResponse r)
                return r.Resource;

            return GetResources(response, configuration).FirstOrDefault();
        }
    }
}
