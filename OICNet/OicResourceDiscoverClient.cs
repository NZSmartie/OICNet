using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using OICNet.CoreResources;

namespace OICNet
{
    public class OicNewDeviceEventArgs : EventArgs
    {
        public OicDevice Device { get; set; }
    }

    public class OicResourceDiscoverClient : IDisposable
    {
        private readonly List<OicDevice> _devices = new List<OicDevice>();

        private readonly List<IOicInterface> _broadcastInterfaces = new List<IOicInterface>();

        private readonly OicConfiguration _configuration;

        public event EventHandler<OicNewDeviceEventArgs> NewDevice;

        public OicResourceDiscoverClient()
            : this(OicConfiguration.Default)
        {

        }

        public OicResourceDiscoverClient(OicConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void AddInterface(IOicInterface provider)
        {
            if (provider is null)
                throw new ArgumentNullException(nameof(provider));
            provider.ReceivedMessage += OnReceivedMessage;
            _broadcastInterfaces.Add(provider);
        }

        private void OnReceivedMessage(object sender, OicReceivedMessageEventArgs e)
        {
            //Todo: Treat responses from multicast requests as being received from a Resource Directory (OIC Core v1.1.1: Section 11.3.6.1.2 Resource directory)
            foreach (var resource in _configuration.Serialiser.Deserialise(e.Message.Payload, e.Message.ContentType))
            {
                OicDevice device;
                if (resource is OicResourceDirectory)
                {
                    OicResourceDirectory directory = resource as OicResourceDirectory;
                    device = _devices.FirstOrDefault(d => d.DeviceId == directory.DeviceId);
                    if (device == null)
                    {
                        device = new OicDevice(_configuration, sender as IOicInterface, e.Endpoint)
                        {
                            DeviceId = directory.DeviceId
                        };
                        NewDevice?.Invoke(this, new OicNewDeviceEventArgs {Device = device});
                    }

                    device.Name = device.Name ?? directory.Name;
                    var newResources = directory.Links.Select(l => l.CreateResource(_configuration.Resolver));
                    if(device.Resources == null)
                        device.Resources = new List<IOicResource>(newResources) ;
                    else 
                        device.Resources.AddRange(newResources);
                }
                else
                {
                    //TODO: match up sender with an interface
                    device = _devices.FirstOrDefault(d => d.RemoteEndpoint.Authority == e.Endpoint.Authority);
                    if (device == null)
                    {
                        device = new OicDevice(_configuration, (sender as IOicInterface), e.Endpoint);
                        NewDevice?.Invoke(this, new OicNewDeviceEventArgs {Device = device});
                    }
                    device.UpdateResourceInternal(resource);
                    break;
                }
            }
        }

        public void Discover()
        {
            // Create a discover request message
            var payload = new OicRequest
            {
                Method = OicMessageMethod.Get,
                Uri = new Uri("/oic/res", UriKind.Relative),
            };


            // Send over transport using multicast/broadcast
            Task.WaitAll(_broadcastInterfaces.Select(transport => 
                Task.Run(async () => await transport.BroadcastMessageAsync(payload))).ToArray());
            
            // Listen for responses
        }

        public void Dispose()
        {
            //TODO: Dispose OICNet.CoreResourcesDiscoverClient properly
        }
    }
}
