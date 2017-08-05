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

        public OicDevice(IOicInterface localInterface, IOicEndpoint remoteEndpoint)
        {
            LocalInterface = localInterface;
            RemoteEndpoint = remoteEndpoint;
        }
        
    }
}
