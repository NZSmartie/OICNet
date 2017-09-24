using System.Threading.Tasks;

namespace OICNet
{
    public abstract class OicClientHandler
    {
        protected OicClientHandler Handler;

        public void SetHandler(OicClientHandler handler) {
            Handler = handler;
        }

        public abstract Task HandleReceivedMessage(OicReceivedMessage message);
    }
}
