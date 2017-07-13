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
        private IOicInterface _interface;
        public IOicInterface Interface
        {
            get { return _interface; }
            set
            {
                _interface = value ?? throw new ArgumentNullException();
                //_interface
            }
        }

        private readonly List<IOicInterface> _broadcastInterfaces;

        EventHandler<OicNewDeviceEventArgs> NewDevice;

        public OicClient()
        {
            _broadcastInterfaces = new List<IOicInterface>();
        }

        public void AddBroadcastInterface(IOicInterface provider)
        {
            _broadcastInterfaces.Add(provider);
        }

        public async Task SendAsync(IOicEndpoint endpoint, OicMessage message)
        {
            await _interface.SendMessageAsync(endpoint, message);
        }

        public void Discover()
        {
            // Create a discover request message
            var payload = new OicMessage
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
