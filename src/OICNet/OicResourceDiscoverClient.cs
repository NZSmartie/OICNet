using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using OICNet.CoreResources;
using System.Diagnostics;

namespace OICNet
{
    public class OicNewDeviceEventArgs : EventArgs
    {
        public OicDevice Device { get; set; }

        public IOicEndpoint Endpoint { get; set; }
    }

    public class OicResourceDiscoverClient : OicClientHandler
    {
        private readonly List<OicRemoteDevice> _devices = new List<OicRemoteDevice>();
        private readonly OicClient _client;
        private readonly OicConfiguration _configuration;

        //Todo: Use INotifyPropertyChanged or IObservableCollection instead of new device event?
        public event EventHandler<OicNewDeviceEventArgs> NewDevice;

        // TODO: make this an exireable cache of request ids
        private readonly List<int> _discoverRequests = new List<int>();

        public OicResourceDiscoverClient(OicClient client)
            : this(client, client.Configuration)
        { }

        public OicResourceDiscoverClient(OicClient client, OicConfiguration configuration)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _configuration = configuration ?? OicConfiguration.Default;

            _client.AddHandler(this);
        }

        public override async Task HandleReceivedMessage(OicReceivedMessage received)
        {
            var isDiscoverResponse = false;

            lock (_discoverRequests)
                isDiscoverResponse = _discoverRequests.Contains(received.Message.RequestId);

            if (!isDiscoverResponse)
            {
                Debug.WriteLine($"Request ({received.Message.RequestId}) was not intended for {nameof(OicResourceDiscoverClient)}");

                if (Handler != null)
                    await Handler.HandleReceivedMessage(received);
                return;
            }

            var response = received.Message as OicResponse;
            Debug.Assert(response != null);

            if(response.ResposeCode != OicResponseCode.Content)
            {
                Debug.WriteLine($"Response to discover request resulted in {response.ResposeCode:G}");
                Debug.WriteLine(_configuration.Serialiser.Prettify(response.Content, response.ContentType));
                return;
            }

            var resources = _configuration.Serialiser.Deserialise(response.Content, response.ContentType) as OicResourceList 
                ?? throw new OicException($"Incorrect response for /oic/res request");

            //Todo: Review Resource Directory (OIC Core v1.1.1: Section 11.3.6.1.2 Resource directory)
            foreach (var resource in resources)
            {
                var newDevice = false;

                if (!(resource is OicResourceDirectory directory))
                {
                    Debug.WriteLine($"{nameof(resource)} ({resource.GetType()}) is not of type {typeof(OicResourceList)}. Skipping");
                    continue;
                }

                OicRemoteDevice device = null;
                lock (_devices)
                    device = _devices.FirstOrDefault(d => d.DeviceId == directory.DeviceId);

                if (device == null)
                {
                    device = new OicRemoteDevice(received.Endpoint)
                    {
                        DeviceId = directory.DeviceId
                    };
                    lock (_devices)
                        _devices.Add(device);
                    newDevice = true;
                }

                device.Name = device.Name ?? directory.Name;
                var newResources = directory.Links.Select(l => l.CreateResource(_configuration.Resolver));
                device.Resources.AddRange(newResources);

                if (newDevice)
                    NewDevice?.Invoke(this, new OicNewDeviceEventArgs { Device = device, Endpoint = received.Endpoint });
            }
        }

        public async Task DiscoverAsync(bool clearCached = false)
        {
            if (clearCached)
                lock(_devices)
                    _devices.Clear();

            // TODO: Cancel previous discovery requests

            // Create a discover request message
            var payload = new OicRequest
            {
                Operation = OicRequestOperation.Get,
                ToUri = new Uri("/oic/res", UriKind.Relative),
            };

            using (var handle = _client.GetHandle(payload))
            {
                lock (_discoverRequests)
                    _discoverRequests.Add(handle.RequestId);

                // Get the handle first and store it before broadcasting request. Reponses may be lost if we don't know our requesting Id.
                await _client.BroadcastAsync(payload);
            }
        }

        public void Dispose()
        {
            //TODO: Dispose OICNet.CoreResourcesDiscoverClient properly
        }
    }
}
