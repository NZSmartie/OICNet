using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

using OICNet;
using OICNet.CoreResources;
using OICNet.Utilities;
using System.Linq;
using System.Reflection;

namespace OICNet.Server.Internal
{
    // this needs to be something that represents /oic/ on a platform/device. 
    public class OicHostDevice : IOicResourceRepository, IDiscoverableResources
    {
        private readonly OicResourceList _resourceDirectory = new OicResourceList();
        private readonly OicConfiguration _configuration;
        private readonly IServiceProvider _services;

        private readonly Dictionary<string, IOicSerialisableResource> _resources = new Dictionary<string, IOicSerialisableResource>(StringComparer.Ordinal);

        private readonly OicDeviceResource _deviceResource = new OicDeviceResource();

        public IReadOnlyList<OicResourceLink> DiscoverableResources { get {
                return _resourceDirectory
                    .Aggregate(Enumerable.Empty<OicResourceLink>(), (a, b) => a.Concat(((OicResourceDirectory)b).Links))
                    .Concat(new[]
                    {
                        OicResourceLink.FromResource(((OicResourceList)_resources["/oic/res"]).First()),
                        OicResourceLink.FromResource((IOicResource)_resources["/oic/d"])
                    })
                    .ToList();
            }
        }

        public OicHostDevice(OicConfiguration configuration, IServiceProvider services)
        {
            _configuration = configuration;
            _services = services;

            _resources.Add("/oic/res", _resourceDirectory);
        }

        public void AddDevice(OicDevice oicDevice)
        {
            if(!_resources.ContainsKey("/oic/d"))
                _resources.Add("/oic/d", new OicDeviceResource
                {
                    SpecVersions = "core.1.1.1", // TODO: Reference this from somewhere (make it a fixed default value?)
                    PlatformId = Guid.NewGuid(),
                    DeviceId = oicDevice.DeviceId,
                    ResourceTypes = oicDevice.DeviceTypes.ToList(),
                    ServerVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion
                });

            var discoveredResources = oicDevice.GetType()
                .GetProperties()
                .Where(p => p.GetCustomAttribute<OicResourceAttribute>() != null)
                .Select(p => Tuple.Create((IOicResource)p.GetMethod.Invoke(oicDevice, null), p.GetCustomAttribute<OicResourceAttribute>(false)));

            foreach (var resource in discoveredResources)
            {
                _resources.Add(resource.Item1.RelativeUri, resource.Item1);

                if (resource.Item2.Policies.HasFlag(OicResourcePolicies.Discoverable))
                    _resourceDirectory.Add(new OicResourceDirectory
                    {
                        DeviceId = oicDevice.DeviceId,
                        Name = oicDevice.Name,
                        Links = discoveredResources
                            .Select(r => new OicResourceLink
                            {
                                Href = new Uri(r.Item1.RelativeUri, UriKind.Relative),
                                ResourceTypes = r.Item1.ResourceTypes,
                                Interfaces = r.Item1.Interfaces,

                                // Inlucde policies if a resource requires secure channel.
                                // TODO: Add observable policy
                                Policies = r.Item2.Policies.HasFlag(OicResourcePolicies.Secure)
                                    ? new OicResourceLink.LinkPolicies
                                    {
                                        Policies = LinkPolicyFlags.Discoverable,
                                        IsSecure = true
                                    }
                                    : null,
                            }).ToList(),
                    });
            }
        }

        private IOicSerialisableResource GetResourceForPath(OicRequest request)
        {
            var path = request.ToUri.AbsolutePath;
            if (_resources.TryGetValue(path, out var resource))
                return resource;

            return null;
        }

        public Task<OicResponse> CreateAsync(OicRequest request, IOicResource resource)
            => Task.FromResult(Create(request, resource));

        public OicResponse Create(OicRequest request, IOicResource resource)
        {
            var oicResource = GetResourceForPath(request);
            if (oicResource != null)
                throw new NotImplementedException();

            return OicResponseUtility.CreateMessage(OicResponseCode.NotFound, "Not found");
        }

        public Task<OicResponse> CreateOrUpdateAsync(OicRequest request, IOicResource resource)
            => Task.FromResult(CreateOrUpdate(request, resource));

        public OicResponse CreateOrUpdate(OicRequest request, IOicResource resource)
        {
            var existing = GetResourceForPath(request);
            if (existing == null)
                return OicResponseUtility.CreateMessage(OicResponseCode.NotFound, "Not found");
            
            if(existing is OicResourceList)
                throw new NotImplementedException();

            ((IOicResource)existing).UpdateFields(resource);
            return new OicResourceResponse(_configuration, existing)
            {
                ResposeCode = OicResponseCode.Changed
            };
        }

        public Task<OicResponse> DeleteAsync(OicRequest request)
            => Task.FromResult(Delete(request));

        public OicResponse Delete(OicRequest request)
        {
            var resource = GetResourceForPath(request);
            if (resource != null)
                throw new NotImplementedException();

            return OicResponseUtility.CreateMessage(OicResponseCode.NotFound, "Not found");
        }

        public Task<OicResponse> RetrieveAsync(OicRequest request)
            => Task.FromResult(Retrieve(request));

        public OicResponse Retrieve(OicRequest request)
        {
            var resource = GetResourceForPath(request);
            if (resource != null)
                return new OicResourceResponse(_configuration, resource) { ResposeCode = OicResponseCode.Content };

            return OicResponseUtility.CreateMessage(OicResponseCode.NotFound, "Not found");
        }
    }
}
