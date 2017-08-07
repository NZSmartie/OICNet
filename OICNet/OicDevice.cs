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

        internal void UpdateResourceInternal(IOicResource resource)
        {
            switch (resource)
            {
                    
            }
        }
    }
}
