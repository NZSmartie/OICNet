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

        public Task<OicResponse> CreateAsync(IOicResource resource)
        {
            throw new NotImplementedException();
        }

        public Task<OicResponse> CreateOrUpdateAsync(IOicResource resource)
        {
            if (resource.RelativeUri == _helloResource.RelativeUri)
            {
                if(resource is OicRemoteResourceRepository request)
                    _helloResource.UpdateFields(request.Resource);
                else
                    _helloResource.UpdateFields(resource);

                return Task.FromResult<OicResponse>(new OicResourceResponse(_configuration, _helloResource)
                {
                    ResposeCode = OicResponseCode.Changed
                });
            }
            throw new NotImplementedException("TODO: return 4.04 not found OicResponse");
        }

        public Task<OicResponse> DeleteAsync(IOicResource resource)
        {
            throw new NotImplementedException();
        }

        public Task<OicResponse> RetrieveAsync(IOicResource resource)
        {
            if (resource.RelativeUri == _helloResource.RelativeUri)
                return Task.FromResult<OicResponse>(new OicResourceResponse(_configuration, _helloResource)
                    {
                        ResposeCode = OicResponseCode.Content
                    });

            throw new NotImplementedException("TODO: return 4.04 not found OicResponse");
        }
    }
}