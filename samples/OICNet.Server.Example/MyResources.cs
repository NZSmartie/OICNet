using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using OICNet.Server.ResourceRepository;
using System.Linq;
using System.Threading.Tasks;
using OICNet.Utilities;

namespace OICNet.Server.Example
{
    [DefaultResourceRepository]
    public class MyResources : IOicResourceRepository
    {
        private readonly OicConfiguration _configuration;

        public IOicResource _helloResource { get; }

        public MyResources(OicConfiguration configuration)
        {
            _configuration = configuration;
            _helloResource = new OicBaseResouece<string>
            {
                Interfaces = OicResourceInterface.Baseline | OicResourceInterface.ReadOnly,
                ResourceTypes = { "oicnet.hello" },
                RelativeUri = "/hello",
                Value = "Hello World"
            };
            
        }

        private IOicResource GetResource(Uri path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            var helloPath = new Uri(path, _helloResource.RelativeUri);
            if (Uri.Compare(path, helloPath, UriComponents.Path, UriFormat.UriEscaped, StringComparison.Ordinal) == 0)
                return _helloResource;

            return null;
        }


        // Async to synchronous proxies

        public Task<OicResponse> CreateAsync(OicRequest request, IOicResource resource)
            => Task.FromResult(Create(request, resource));

        public Task<OicResponse> CreateOrUpdateAsync(OicRequest request, IOicResource resource)
            => Task.FromResult(CreateOrUpdate(request, resource));

        public Task<OicResponse> DeleteAsync(OicRequest request)
            => Task.FromResult(Delete(request));

        public Task<OicResponse> RetrieveAsync(OicRequest request)
            => Task.FromResult(Retrieve(request));


        public OicResponse Create(OicRequest request, IOicResource resource)
        {
            var myResource = GetResource(request.ToUri);
            if (myResource == null)
                return OicResponseUtility.CreateMessage(OicResponseCode.NotFound, "Resource not found");

            throw new NotImplementedException();
        }

        public OicResponse CreateOrUpdate(OicRequest request, IOicResource resource)
        {
            var myResource = GetResource(request.ToUri);
            if (myResource == null)
                return OicResponseUtility.CreateMessage(OicResponseCode.NotFound, "Resource not found");

            if ((myResource.Interfaces & OicResourceInterface.ReadWrite) == OicResourceInterface.ReadWrite)
                return OicResponseUtility.CreateMessage(OicResponseCode.OperationNotAllowed, "Operation not allowed");

            if (resource == null)
                return OicResponseUtility.CreateMessage(OicResponseCode.BadRequest, "No valid OIC resource was provided");

            myResource.UpdateFields(resource);

            return new OicResourceResponse(_configuration, _helloResource)
            {
                ResposeCode = OicResponseCode.Changed
            };

        }

        public OicResponse Delete(OicRequest request)
        {
            var myResource = GetResource(request.ToUri);
            if (myResource == null)
                return OicResponseUtility.CreateMessage(OicResponseCode.NotFound, "Resource not found");

            throw new NotImplementedException();
        }

        public OicResponse Retrieve(OicRequest request)
        {
            var myResource = GetResource(request.ToUri);
            if (myResource == null)
                return OicResponseUtility.CreateMessage(OicResponseCode.NotFound, "Resource not found");

            return new OicResourceResponse(_configuration, myResource)
            {
                ResposeCode = OicResponseCode.Content
            };
        }
    }
}