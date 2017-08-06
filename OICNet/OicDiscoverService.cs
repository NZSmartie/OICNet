using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace OICNet
{
    public class OicNewDeviceEventArgs : EventArgs
    {
        public OicDevice Device { get; set; }
    }

    public class OicDiscoverService : IDisposable
    {
        private readonly List<OicDevice> _devices = new List<OicDevice>();

        private readonly List<IOicInterface> _broadcastInterfaces = new List<IOicInterface>();

        private readonly OicConfiguration _configuration;

        public event EventHandler<OicNewDeviceEventArgs> NewDevice;

        public OicDiscoverService()
            : this(OicConfiguration.Default)
        {

        }

        public OicDiscoverService(OicConfiguration configuration)
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
            //TODO: match up sender with an interface
            var device = _devices.FirstOrDefault(d => d.RemoteEndpoint.Authority == e.Endpoint.Authority);
            if (device == null)
            {
                device = new OicDevice(_configuration, (sender as IOicInterface), e.Endpoint);
                NewDevice?.Invoke(this, new OicNewDeviceEventArgs { Device = device });
            }

            device.PassMessage(e.Message);
            //_serialiser.Deserialise(e.Message.Payload, e.Message.ContentType);

            //ReceivedMessage?.Invoke(this, new OicDeviceReceivedMessageEventArgs
            //{
            //    Device = device,
            //    Message = e.Message
            //});
        }

        public void Discover()
        {
            // Create a discover request message
            var payload = new OicRequest
            {
                Method = OicMessageMethod.Get,
                Uri = "/oic/res",
            };


            // Send over transport using multicast/broadcast
            Task.WaitAll(_broadcastInterfaces.Select(transport => 
                Task.Run(async () => await transport.BroadcastMessageAsync(payload))).ToArray());
            
            // Listen for responses
        }

        public void Dispose()
        {
            //TODO: Dispose OICNet.OicDiscoverService properly
        }
    }
}
