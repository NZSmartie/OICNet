using CoAPNet;
using CoAPNet.Udp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OICNet.Server.Builder;
using OICNet.Server.Hosting;

namespace OICNet.Server.Example
{
    class program
    {
        static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddCommandLine(args)
                .Build();


            var host = new OicHostBuilder()
                .UseConfiguration(config)
                .ConfigureLogging(logging =>
                {
                    logging
                        .AddDebug()
                        .AddConsole();
                })
                .UseCoap(options =>
                {
                    // TODO: Allow providing listening options from IOptions<OicCoapServer>
                    options.Listen(new CoapUdpEndPoint(Coap.Port));

                    // enable /.well-know/core
                    options.UseCoreLink = true;
                })
                .UseCoapUdp()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
