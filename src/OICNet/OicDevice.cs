using System;
using System.Collections.Generic;
using System.Linq;
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

        public Guid DeviceId { get; set; }

        public List<IOicResource> Resources { get; } = new List<IOicResource>();

        public IOicEndpoint Endpoint { get; }

        public OicConfiguration Configuration { get; }

        public OicDevice(IOicEndpoint remoteEndpoint)
            :this(remoteEndpoint, OicConfiguration.Default)
        {
            
        }

        public OicDevice(IOicEndpoint endpoint, OicConfiguration configuration)
        {
            Configuration = configuration;
            Endpoint = endpoint;
        }

        internal void UpdateResourceInternal(IOicResource resource)
        {
            switch (resource)
            {
                    
            }
        }
    }
}
