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

        private readonly List<IOicTransport> _broadcastTransports = new List<IOicTransport>();

        private readonly OicConfiguration _configuration;

        //Todo: Use INotifyPropertyChanged or IObservableCollection instead of new device event?
        public event EventHandler<OicNewDeviceEventArgs> NewDevice;

        public OicResourceDiscoverClient()
            : this(OicConfiguration.Default)
        {

        }

        public OicResourceDiscoverClient(OicConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void AddTransport(IOicTransport provider)
        {
            if (provider is null)
                throw new ArgumentNullException(nameof(provider));
            provider.ReceivedMessage += OnReceivedMessage;
            _broadcastTransports.Add(provider);
        }

        private void OnReceivedMessage(object sender, OicReceivedMessageEventArgs e)
        {
            var message = e.Message as OicResponse ?? throw new InvalidOperationException();

            //Todo: Treat responses from multicast requests as being received from a Resource Directory (OIC Core v1.1.1: Section 11.3.6.1.2 Resource directory)
            foreach (var resource in _configuration.Serialiser.Deserialise(e.Message.Content, message.ContentType))
            {
                OicDevice device;
                if (resource is OicResourceDirectory)
                {
                    OicResourceDirectory directory = resource as OicResourceDirectory;
                    device = _devices.FirstOrDefault(d => d.DeviceId == directory.DeviceId);
                    if (device == null)
                    {
                        device = new OicDevice(e.Endpoint, _configuration)
                        {
                            DeviceId = directory.DeviceId
                        };
                        _devices.Add(device);
                        NewDevice?.Invoke(this, new OicNewDeviceEventArgs {Device = device});
                    }

                    device.Name = device.Name ?? directory.Name;
                    var newResources = directory.Links.Select(l =>
                    {
                        var r = l.CreateResource(_configuration.Resolver);
                        r.Device = device;
                        return r;
                    });
                    device.Resources.AddRange(newResources);
                }
                else
                {
                    //TODO: Verify this will correctly match up devices
                    device = _devices.FirstOrDefault(d => d.Endpoint.Transport == e.Endpoint.Transport && d.Endpoint.Authority == e.Endpoint.Authority);
                    if (device == null)
                    {
                        device = new OicDevice(e.Endpoint, _configuration);
                        _devices.Add(device);
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
                Operation = OicRequestOperation.Get,
                ToUri = new Uri("/oic/res", UriKind.Relative),
            };


            // Send over transport using multicast/broadcast
            Task.WaitAll(_broadcastTransports.Select(transport => 
                Task.Run(async () => await transport.BroadcastMessageAsync(payload))).ToArray());
            
            // Listen for responses
        }

        public void Dispose()
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            _broadcastTransports.ForEach(t => (t as IDisposable)?.Dispose());

            //TODO: Dispose OICNet.CoreResourcesDiscoverClient properly
        }
    }
}
