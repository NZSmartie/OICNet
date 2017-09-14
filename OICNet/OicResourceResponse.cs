using System;
using System.Collections.Generic;
using System.Linq;

namespace OICNet
{
    /// <summary>
    /// Simple wrapper for <see cref="IOicResource"/> to be used as a Response in a <see cref="OicContext"/>
    /// </summary>
    public class OicResourceResponse : OicResponse
    {
        private readonly OicConfiguration _configuration;
        private readonly IOicResource _resource;

        public IOicResource Resource => _resource;

        public override byte[] Content
        {
            get => _configuration.Serialiser.Serialise(_resource, ContentType);
            set => throw new InvalidOperationException();
        }

        public OicResourceResponse(OicConfiguration configuration, IOicResource resource)
        {
            _configuration = configuration;
            _resource = resource;
        }
    }
}