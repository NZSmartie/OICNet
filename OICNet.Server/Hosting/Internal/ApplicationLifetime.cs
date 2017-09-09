using System;
using System.Threading;

namespace OICNet.Server.Hosting.Internal
{
    public class ApplicationLifetime : IApplicationLifetime
    {
        private readonly CancellationTokenSource _startedSource = new CancellationTokenSource();
        private readonly CancellationTokenSource _stoppingSource = new CancellationTokenSource();
        private readonly CancellationTokenSource _stoppedSource = new CancellationTokenSource();

        public CancellationToken ApplicationStarted => _startedSource.Token;

        public CancellationToken ApplicationStopping => _stoppingSource.Token;

        public CancellationToken ApplicationStopped => _stoppedSource.Token;

        public void NotifyStarted()
        {
            try
            {
                _startedSource.Cancel(false);
            }
            catch (Exception)
            {
                
            }
        }

        public void NotifyStopped()
        {
            try
            {
                _stoppedSource.Cancel(false);
            }
            catch (Exception)
            {

            }
        }

        public void StopApplication()
        {
            lock (_stoppingSource)
            {
                try
                {
                    _stoppingSource.Cancel(false);
                }
                catch (Exception)
                {

                }
            }
        }
    }
}