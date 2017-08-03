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

    public class OicClient : IDisposable
    {
        private readonly List<OicDevice> _devices = new List<OicDevice>();

        private readonly List<IOicInterface> _broadcastInterfaces;

        private readonly OicMessageSerialiser _serialiser;

        public event EventHandler<OicNewDeviceEventArgs> NewDevice;

        public event EventHandler<OicDeviceReceivedMessageEventArgs> ReceivedMessage;


        public OicClient()
        {
            _broadcastInterfaces = new List<IOicInterface>();

            _serialiser = new OicMessageSerialiser(new OicResolver());
        }

        public void AddBroadcastInterface(IOicInterface provider)
        {
            if (provider is null)
                throw new ArgumentNullException();
            provider.ReceivedMessage += OnReceivedMessage;
            _broadcastInterfaces.Add(provider);
        }

        public async Task SendAsync(OicDevice device, OicRequest message)
        {
            await device.LocalInterface.SendMessageAsync(device.RemoteEndpoint, message);
        }

        private void OnReceivedMessage(object sender, OicReceivedMessageEventArgs e)
        {
            //TODO: match up sender with an interface
            System.Diagnostics.Debug.WriteLine($"Got a message from {e.Endpoint.Authority}");

            var device = _devices.FirstOrDefault(d => d.RemoteEndpoint.Authority == e.Endpoint.Authority);
            if (device == null)
            {
                device = new OicDevice((sender as IOicInterface), e.Endpoint);
                NewDevice?.Invoke(this, new OicNewDeviceEventArgs { Device = device });
            }

            System.Diagnostics.Debugger.Break();
            if(e.Message.ContentType == OicMessageContentType.ApplicationJson)
            {
                var result = _serialiser.Deserialise(e.Message.Payload, OicMessageContentType.ApplicationJson);
            }

            ReceivedMessage?.Invoke(this, new OicDeviceReceivedMessageEventArgs
            {
                Device = device,
                Message = e.Message
            });
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
            //TODO: Dispose OICNet.OicClient properly
        }
    }
}
