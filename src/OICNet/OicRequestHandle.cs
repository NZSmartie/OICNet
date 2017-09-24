using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OICNet
{
    public class OicRequestResponseEventArgs : EventArgs
    {
        public OicMessage Message { get; set; }
    }

    public class OicRequestHandle : IDisposable
    {
        private readonly OicClient _client;

        private TaskCompletionSource<bool> _responseTcs = new TaskCompletionSource<bool>();

        private object _reponseLock = new object();
        private OicMessage _response;
        public OicMessage Response
        {
            get
            {
                lock (_reponseLock)
                {
                    if (_response != null)
                        return _response;
                }

                _responseTcs.Task.GetAwaiter().GetResult();

                lock (_reponseLock)
                    return _response;
            }
        }

        public event EventHandler<OicRequestResponseEventArgs> OnResponse;

        public int RequestId { get; }

        internal OicRequestHandle(OicClient client,int requestId)
        {
            _client = client;
            RequestId = requestId;
        }

        internal Task SetReponseAsync(OicMessage message)
        {
            lock(_reponseLock)
                _response = message;

            _responseTcs.TrySetResult(true);
            return Task.Run(() => OnResponse?.Invoke(this, new OicRequestResponseEventArgs { Message = message }));
        }

        public async Task<OicMessage> GetReponseAsync()
        {
            await _responseTcs.Task;

            lock (_reponseLock)
                return _response;
        }

        public void Dispose()
        {
            _client.RemoveHandle(this);
        }
    }
}
