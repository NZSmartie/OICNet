using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using OICNet.Server.ResourceRepository;
using System.Linq;
using System.Threading.Tasks;
using OICNet.Utilities;

namespace OICNet.Server.Example
{
    public class MyOtherResources : IOicResourceRepository
    {
        private readonly OicConfiguration _configuration;

        public IOicResource _helloResource { get; }

        public MyOtherResources(OicConfiguration configuration, string helloPath)
        {
            _configuration = configuration;
            _helloResource = new OicBaseResouece<string>
            {
                Interfaces = { OicResourceInterface.Baseline, OicResourceInterface.ReadOnly },
                ResourceTypes = { "oicnet.hello" },
                RelativeUri = helloPath,
                Value = "Hello You! You're Awesome!"
            };
            
        }

        private IOicResource GetResource(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            if (_helloResource.RelativeUri.Equals(path, StringComparison.Ordinal))
                return _helloResource;

            return null;
        }


        // Async to synchronous proxies

        public Task<OicResponse> CreateAsync(string path, IOicResource resource)
            => Task.FromResult(Create(path, resource));

        public Task<OicResponse> CreateOrUpdateAsync(string path, IOicResource resource)
            => Task.FromResult(CreateOrUpdate(path, resource));

        public Task<OicResponse> DeleteAsync(string path)
            => Task.FromResult(Delete(path));

        public Task<OicResponse> RetrieveAsync(string path)
            => Task.FromResult(Retrieve(path));


        public OicResponse Create(string path, IOicResource resource)
        {
            var myResource = GetResource(path);
            if (myResource == null)
                return OicResponseUtility.CreateMessage(OicResponseCode.NotFound, "Resource not found");

            throw new NotImplementedException();
        }

        public OicResponse CreateOrUpdate(string path, IOicResource resource)
        {
            var myResource = GetResource(path);
            if (myResource == null)
                return OicResponseUtility.CreateMessage(OicResponseCode.NotFound, "Resource not found");

            if (!myResource.Interfaces.Contains(OicResourceInterface.ReadWrite))
                return OicResponseUtility.CreateMessage(OicResponseCode.OperationNotAllowed, "Operation not allowed");

            if (resource == null)
                return OicResponseUtility.CreateMessage(OicResponseCode.BadRequest, "No valid OIC resource was provided");

            myResource.UpdateFields(resource);

            return new OicResourceResponse(_configuration, _helloResource)
            {
                ResposeCode = OicResponseCode.Changed
            };

        }

        public OicResponse Delete(string path)
        {
            var myResource = GetResource(path);
            if (myResource == null)
                return OicResponseUtility.CreateMessage(OicResponseCode.NotFound, "Resource not found");

            throw new NotImplementedException();
        }

        public OicResponse Retrieve(string path)
        {
            var myResource = GetResource(path);
            if (myResource == null)
                return OicResponseUtility.CreateMessage(OicResponseCode.NotFound, "Resource not found");

            return new OicResourceResponse(_configuration, myResource)
            {
                ResposeCode = OicResponseCode.Content
            };
        }
    }
}