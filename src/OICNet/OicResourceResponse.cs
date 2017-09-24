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
        private readonly IOicSerialisableResource _resource;

        public bool IsCollection => _resource is OicResourceList;

        public OicResourceList Resources 
            => _resource != null 
                ? (_resource as OicResourceList ?? new OicResourceList(new[] { Resource })) 
                : null;

        public IOicResource Resource 
            => _resource != null 
            ? _resource as IOicResource ?? Resources.First()
            : null;

        public override byte[] Content
        {
            get => _configuration.Serialiser.Serialise(_resource, ContentType);
            set => throw new InvalidOperationException();
        }

        public OicResourceResponse(OicConfiguration configuration, IOicSerialisableResource resource)
        {
            _configuration = configuration;
            _resource = resource;

            // Set default content type
            ContentType = OicMessageContentType.ApplicationCbor;
        }
    }
}