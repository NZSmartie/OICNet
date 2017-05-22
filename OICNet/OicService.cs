using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace OICNet
{
    public class OicService
    {
        List<IOicTransportProvider> _transportProviders;

        public OicService()
        {
            _transportProviders = new List<IOicTransportProvider>();
        }

        public void AddTransportProvider(IOicTransportProvider provider)
        {
            _transportProviders.Add(provider);
        }

        public void Discover()
        {
            // Create a discover request message
            var payload = new OicMessage
            {
                Method = Method.Get,
                Uri = "/oic/res",
            };

            // Send over transport using multicast/broadcast
            foreach(var transportProvider in _transportProviders)
                transportProvider.GetBroadcasters().ForEach(b => b.SendBroadcastAsync(payload));


            // Listen for responses

        }
    }
}
