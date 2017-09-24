using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OICNet
{
    public class OicClient : IDisposable
    {
        private readonly List<IOicTransport> _transports = new List<IOicTransport>();
        public readonly OicConfiguration Configuration;

        private readonly List<Task> _receiveTasks = new List<Task>();
        private readonly CancellationTokenSource _receiveCTS = new CancellationTokenSource();

        private OicClientHandler _handler = null;
        private OicClientHandler _prevHandler = null;

        private readonly Dictionary<int, OicRequestHandle> _requestHandlers = new Dictionary<int, OicRequestHandle>();

        private int _requestId;

        //Todo: Use INotifyPropertyChanged or IObservableCollection instead of new device event?
        public OicClient()
            : this(OicConfiguration.Default)
        { }

        internal void RemoveHandle(OicRequestHandle handle)
        {
            lock (_requestHandlers)
                _requestHandlers.Remove(handle.RequestId);
        }

        public OicClient(OicConfiguration configuration)
        {
            Configuration = configuration;

            _requestId = new Random().Next();
        }

        private int GetNextRequestId()
            => Interlocked.Increment(ref _requestId) & int.MaxValue;

        private OicRequestHandle CreateHandle(OicMessage message)
        {
            if (message.RequestId == 0)
                message.RequestId = GetNextRequestId();

            var handle = new OicRequestHandle(this, message.RequestId);

            lock (_requestHandlers)
                _requestHandlers.Add(handle.RequestId, handle);

            return handle;
        }

        public async Task<OicRequestHandle> SendAsync(OicMessage message, IOicEndpoint endpoint = null)
        {
            var handle = CreateHandle(message);

            if (endpoint != null)
                await endpoint.Transport.SendMessageAsync(message, endpoint);
            else
                await Task.WhenAny(_transports.Select(t => t.SendMessageAsync(message)));

            return handle;
        }

        public void AddHandler(OicClientHandler handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            if (_handler == null)
                _handler = handler;

            _prevHandler?.SetHandler(handler);
            _prevHandler = handler;
        }

        public async Task<OicRequestHandle> BroadcastAsync(OicMessage message)
        {
            var handle = CreateHandle(message);

            await Task.WhenAll(_transports.Select(t => t.BroadcastMessageAsync(message)));

            return handle;
        }

        public void AddTransport(IOicTransport provider)
        {
            if (provider is null)
                throw new ArgumentNullException(nameof(provider));
            _transports.Add(provider);

            lock (_receiveTasks)
            {
                var receiveTask = Task.Factory.StartNew(
                    ReceiveInternalAsync,
                    provider,
                    _receiveCTS.Token,
                    TaskCreationOptions.RunContinuationsAsynchronously,
                    TaskScheduler.Default);

                _receiveTasks.Add(receiveTask.Result);
            }
        }

        public async Task ReceiveInternalAsync(object state)
        {
            var transport = state as IOicTransport ?? throw new ArgumentException(nameof(state));
            var ct = _receiveCTS.Token;
            try
            {
                while (!ct.IsCancellationRequested)
                {
                    var received = await transport.ReceiveMessageAsync(ct);
                    // Middle ware sort of callback?

                    Debug.WriteLine($"received a message from {received.Endpoint}");

                    OicRequestHandle requestHandler = null;
                    lock (_requestHandlers)
                        _requestHandlers.TryGetValue(received.Message.RequestId, out requestHandler);

                    _ = requestHandler?.SetReponseAsync(received.Message);

                    _ = Task.Factory.StartNew(
                        async r => await _handler.HandleReceivedMessage((OicReceivedMessage)r), 
                        received, 
                        ct, 
                        TaskCreationOptions.RunContinuationsAsynchronously, 
                        TaskScheduler.Default);
                }
            }
            catch(Exception ex)
            {
                if (ct.IsCancellationRequested)
                    return;
                Debug.WriteLine($"Exception occured in {nameof(ReceiveInternalAsync)}");
                Debug.Write(ex);
            }
        }

        public void Dispose()
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            _transports.ForEach(t => (t as IDisposable)?.Dispose());

            _receiveCTS.Cancel();
            Task.WaitAll(_receiveTasks.ToArray());

            //TODO: Dispose OICNet.CoreResourcesDiscoverClient properly
        }
    }
}
