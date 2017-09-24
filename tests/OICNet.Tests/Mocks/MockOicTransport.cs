using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Nito.AsyncEx;

namespace OICNet.Tests.Mocks
{
    public class MockOicTransport : IOicTransport
    {
        internal bool IsDisposed = false;

        private readonly Queue<OicReceivedMessage> _receiveQueue = new Queue<OicReceivedMessage>();
        private readonly AsyncAutoResetEvent _receiveEnqueuedEvent = new AsyncAutoResetEvent(false);

        public void Dispose()
        {
            IsDisposed = true;
            _receiveEnqueuedEvent.Set();
        }

        public Task<int> SendMessageAsync(OicMessage message, IOicEndpoint endpoint = null)
        {
            return IsDisposed
                ? throw new OicException("Encdpoint Disposed")
                : MockSendMessageAsync(message);
        }

        public virtual Task<int> MockSendMessageAsync(OicMessage packet)
        {
            return Task.FromResult(0);
        }

        public void EnqueueReceivePacket(OicReceivedMessage messages)
        {
            lock (_receiveQueue)
            {
                _receiveQueue.Enqueue(messages);
            }
            _receiveEnqueuedEvent.Set();
        }

        public virtual Task<OicReceivedMessage> ReceiveMessageAsync(CancellationToken token)
        {
            return IsDisposed
                ? throw new OicException("Encdpoint Disposed")
                : MockReceiveMessageAsync(token);
        }

        public virtual async Task<OicReceivedMessage> MockReceiveMessageAsync(CancellationToken token)
        {
            await _receiveEnqueuedEvent.WaitAsync(token);
            if (IsDisposed)
                throw new OicException("Encdpoint Disposed");

            OicReceivedMessage packet;
            lock (_receiveQueue)
            {
                packet = _receiveQueue.Dequeue();
            }
            return packet;
        }

        public virtual Task BroadcastMessageAsync(OicMessage message)
        {
            return Task.CompletedTask;
        }
    }
}
