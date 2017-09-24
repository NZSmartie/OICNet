using CoAPNet.Udp;
using OICNet.CoAP;
using System;

namespace OICNet.ClientExample
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var client = new OicClient())
            {
                // Add a CoapTransport with a system assigned UDP Endpoint
                client.AddTransport(new OicCoapTransport(new CoapUdpEndPoint()));

                // Create a discover client
                var discoverClient = new OicResourceDiscoverClient(client);
                discoverClient.NewDevice += OnNewDevice;

                // Broadcast a discover request (GET /oic/res)
                discoverClient.Discover().Wait();

                Console.WriteLine("Press <Enter> to exit");
                Console.ReadLine();
            }
        }

        private static void OnNewDevice(object sender, OicNewDeviceEventArgs e)
        {
            Console.WriteLine($"New device found \"{e.Device.Name}\" ({e.Device.DeviceId})");
            foreach(var resource in e.Device.Resources)
            {
                Console.WriteLine($" - {resource.RelativeUri}\n\tName: {resource.Name}\n\tResource Types: {string.Join(", ", resource.ResourceTypes)}");
            }
        }
    }
}
