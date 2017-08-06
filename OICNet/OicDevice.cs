using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OICNet
{
    public class OicDeviceReceivedMessageEventArgs : EventArgs
    {
        public OicDevice Device;

        public OicMessage Message;
    }

    public class OicDevice
    {
        public string Name { get; set; }

        public List<IOicResource> Resources { get; set; }

        public IOicInterface LocalInterface{ get; }

        public IOicEndpoint RemoteEndpoint { get; }

        private readonly OicConfiguration _configuration;

        public OicDevice(IOicInterface localInterface, IOicEndpoint remoteEndpoint)
            :this(OicConfiguration.Default, localInterface, remoteEndpoint)
        {
            
        }

        public OicDevice(OicConfiguration configuration, IOicInterface localInterface, IOicEndpoint remoteEndpoint)
        {
            _configuration = configuration;
            LocalInterface = localInterface;
            RemoteEndpoint = remoteEndpoint;
        }

        internal void PassMessage(OicResponse response)
        {
            //var resource = _configuration.Serialiser.Deserialise(response.Payload, response.ContentType);
            //if(response.Uri.Equals("/oic/res",StringComparison.OrdinalIgnoreCase))
            //    resource.
        }
    }
}
