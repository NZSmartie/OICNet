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
    public class OicHostDevice : IOicResourceRepository
    {
        private readonly IList<IOicResource> _resources = new List<IOicResource>();

        private readonly OicResourceList _resourceDirectory = new OicResourceList();
        private readonly OicConfiguration _configuration;
        private readonly IServiceProvider _services;
        private readonly OicDevice _device;

        private readonly OicDeviceResource _deviceResource = new OicDeviceResource();

        public OicHostDevice(OicConfiguration configuration, IServiceProvider services, OicDevice device)
        {
            _configuration = configuration;
            _services = services;

            _device = device;

            _deviceResource = new OicDeviceResource
            {
                SpecVersions = "core.1.1.1", // TODO: Reference this from somewhere (make it a fixed default value?)
                PlatformId = Guid.NewGuid(),
                DeviceId = _device.DeviceId,
                //ResourceTypes = _device.GetType().GetCustomAttributes(typeof(OicDeviceTypeAttribute), false).Select(a => ((OicDeviceTypeAttribute)a).Type).Concat(new[] { "oic.wk.d" }).ToList(),
                ResourceTypes = _device.DeviceTypes.ToList(),
                ServerVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion
            };

            var discoverableResources = _device.GetType()
                .GetProperties()
                .Where(p => p.GetCustomAttributes<OicResourceAttribute>(false).Any())
                .Select(p => Tuple.Create((IOicResource)p.GetMethod.Invoke(_device, null), p.GetCustomAttributes<OicResourceAttribute>(false)));

            _resourceDirectory.Add(new OicResourceDirectory
            {
                DeviceId = _device.DeviceId,
                Name = _device.Name,
                Links = discoverableResources
                    .Select(r => new OicResourceLink
                    {
                        Href = new Uri(r.Item1.RelativeUri, UriKind.Relative),
                        ResourceTypes = r.Item1.ResourceTypes,
                        Interfaces = r.Item1.Interfaces,

                        // Inlucde policies if a resource requires secure channel.
                        // TODO: Add observable policy
                        Policies = r.Item2.Any(a => (a.Policies & OicResourcePolicies.Secure) != OicResourcePolicies.None)
                            ? new OicResourceLink.LinkPolicies
                            {
                                Policies = LinkPolicyFlags.Discoverable,
                                IsSecure = true
                            }
                            : null,
                    }).ToList(),
            });
        }

        private IOicSerialisableResource GetResourceForPath(string path)
        {
            if ("/res".Equals(path, StringComparison.Ordinal))
                return _resourceDirectory;

            if ("/d".Equals(path, StringComparison.Ordinal))
                return _deviceResource;

            return null;
        }

        public Task<OicResponse> CreateAsync(string path, IOicResource resource)
            => Task.FromResult(Create(path, resource));

        public OicResponse Create(string path, IOicResource resource)
        {
            var oicResource = GetResourceForPath(path);
            if (oicResource != null)
                throw new NotImplementedException();

            return OicResponseUtility.CreateMessage(OicResponseCode.NotFound, "Not found");
        }

        public Task<OicResponse> CreateOrUpdateAsync(string path, IOicResource resource)
            => Task.FromResult(CreateOrUpdate(path, resource));

        public OicResponse CreateOrUpdate(string path, IOicResource resource)
        {
            var oicResource = GetResourceForPath(path);
            if (oicResource != null)
                throw new NotImplementedException();

            return OicResponseUtility.CreateMessage(OicResponseCode.NotFound, "Not found");
        }

        public Task<OicResponse> DeleteAsync(string path)
            => Task.FromResult(Delete(path));

        public OicResponse Delete(string path)
        {
            var resource = GetResourceForPath(path);
            if (resource != null)
                throw new NotImplementedException();

            return OicResponseUtility.CreateMessage(OicResponseCode.NotFound, "Not found");
        }

        public Task<OicResponse> RetrieveAsync(string path)
            => Task.FromResult(Retrieve(path));

        public OicResponse Retrieve(string path)
        {
            var resource = GetResourceForPath(path);
            if (resource != null)
                return new OicResourceResponse(_configuration, resource) { ResposeCode = OicResponseCode.Content };

            return OicResponseUtility.CreateMessage(OicResponseCode.NotFound, "Not found");
        }
    }
}
