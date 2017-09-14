using System;
using System.Collections.Generic;
using System.Linq;

namespace OICNet
{
    public class OicResourceRequest : IOicResource
    {
        private readonly OicConfiguration _configuration;
        private readonly OicRequest _request;
        private readonly string _relativeUri;

        private IOicResource _resource;
        public IOicResource Resource => _resource 
            ?? (_resource = _request.ContentType != OicMessageContentType.None 
                ? _configuration.Serialiser.Deserialise(_request.Content, _request.ContentType).First() 
                : null);

        public OicResourceRequest(OicConfiguration configuration, OicRequest request, string relativeUri = null)
        {
            _configuration = configuration;
            _request = request;
            _relativeUri = relativeUri;
        }

        public string RelativeUri { get => _relativeUri ?? _request.ToUri.AbsolutePath; set => throw new NotSupportedException(); }

        public string Id { get => Resource?.Id; set => throw new NotSupportedException(); }

        public IList<OicResourceInterface> Interfaces => Resource?.Interfaces;

        public string Name { get => Resource?.Name; set => throw new NotSupportedException(); }

        public IList<string> ResourceTypes => Resource?.ResourceTypes;

        public void UpdateFields(IOicResource source)
        {
            throw new NotImplementedException();
        }
    }
}