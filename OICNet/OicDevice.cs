using System;
using System.Collections.Generic;
using System.Text;

namespace OICNet
{
    public class OicDevice
    {
        public string Name { get; set; }

        public List<IOicResource> Resources { get; set; }

        public IOicEndpoint Endpoint { get; }

        public OicDevice(IOicEndpoint endpoint)
        {
            Endpoint = endpoint;
        }
    }
}
