using System;

namespace OICNet.Server
{
    /// <summary>
    /// Simple wrapper for <see cref="IOicResource"/> to be used as a Response in a <see cref="OicContext"/>
    /// </summary>
    public class OicResourceResponse : OicResponse
    {
        private readonly OicConfiguration _configuration;
        private readonly IOicResource _resource;
        public override OicMessageContentType ContentType { get; set; }

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