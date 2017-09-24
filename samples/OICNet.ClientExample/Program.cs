using CoAPNet.Udp;
using OICNet.CoAP;
using System;
using System.Diagnostics;
using System.Linq;

namespace OICNet.ClientExample
{
    class Program
    {
        private static OicClient _client;

        static void Main(string[] args)
        {
            using (_client = new OicClient())
            {
                // Add a CoapTransport with a system assigned UDP Endpoint
                _client.AddTransport(new OicCoapTransport(new CoapUdpEndPoint()));

                // Create a discover client
                var discoverClient = new OicResourceDiscoverClient(_client);
                discoverClient.NewDevice += OnNewDevice;

                // Broadcast a discover request (GET /oic/res)
                discoverClient.Discover().Wait();

                Console.WriteLine("Press <Enter> to exit");
                Console.ReadLine();
            }
        }

        private static void OnNewDevice(object sender, OicNewDeviceEventArgs e)
        {
            try
            {

                Console.WriteLine($"New device found \"{e.Device.Name}\" ({e.Device.DeviceId})");

                var resourceRepository = new OicRemoteResourceRepository(e.Device as OicRemoteDevice, _client);

                foreach (var resource in e.Device.Resources)
                {
                    Console.WriteLine($" - {resource.RelativeUri}\n\tName: {resource.Name}\n\tResource Types: {string.Join(", ", resource.ResourceTypes)}");

                    var response = resourceRepository.RetrieveAsync(OicRequest.Create(resource.RelativeUri)).Result;
                    resource.UpdateFields(response.Resource);
                    Console.WriteLine($"\tValue: {resource}");
                }
            }
            catch (AggregateException aex)
            {
                Debugger.Break();
            }
        }
    }
}
