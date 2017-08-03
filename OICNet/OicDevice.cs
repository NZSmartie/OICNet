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

        public async Task<IOicResource> GetAsync(string uri, string[] filters)
        {
            var response = await LocalInterface.SendMessageWithResponseAsync(RemoteEndpoint, new OicRequest
            {
                Uri = uri,
                Method = OicMessageMethod.Get,
                Filters = new List<string>(filters),
            });
    
            

            //Todo: return result
            return await Task.FromResult<IOicResource>(null);
        }

        public Task PutAsync(string uri, IOicResource resource)
        {
            throw new NotImplementedException();
        }

        public Task PostAsync(string uri, IOicResource resource)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(string uri)
        {
            throw new NotImplementedException();
        }

        public Task RequestAsync(OicRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
