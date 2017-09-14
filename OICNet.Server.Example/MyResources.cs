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
                Interfaces = { OicResourceInterface.Baseline, OicResourceInterface.ReadOnly },
                ResourceTypes = { "oicnet.hello" },
                RelativeUri = "/hello",
                Value = "Hello World"
            };
            
        }

        public Task<OicResponse> CreateAsync(string path, IOicResource resource)
        {
            throw new NotImplementedException();
        }

        public Task<OicResponse> CreateOrUpdateAsync(string path, IOicResource resource)
        {
            if(resource == null)
                return Task.FromResult(OicResponseUtility.CreateMessage(OicResponseCode.BadRequest, "No valid OIC resource was provided"));

            if (_helloResource.RelativeUri.Equals(path, StringComparison.Ordinal))
            {
                _helloResource.UpdateFields(resource);

                return Task.FromResult<OicResponse>(new OicResourceResponse(_configuration, _helloResource)
                {
                    ResposeCode = OicResponseCode.Changed
                });
            }
            return Task.FromResult(OicResponseUtility.CreateMessage(OicResponseCode.NotFound, "Resource not found"));
        }

        public Task<OicResponse> DeleteAsync(string path)
        {
            throw new NotImplementedException();
        }

        public Task<OicResponse> RetrieveAsync(string path)
        {
            if (_helloResource.RelativeUri.Equals(path, StringComparison.Ordinal))
                return Task.FromResult<OicResponse>(new OicResourceResponse(_configuration, _helloResource)
                    {
                        ResposeCode = OicResponseCode.Content
                    });

            return Task.FromResult(OicResponseUtility.CreateMessage(OicResponseCode.NotFound, "Resource not found"));
        }
    }
}