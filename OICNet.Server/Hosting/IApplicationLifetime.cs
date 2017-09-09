using System.Threading;

namespace OICNet.Server.Hosting
{
    internal interface IApplicationLifetime
    {
        CancellationToken ApplicationStarted { get; }

        CancellationToken ApplicationStopping { get; }

        CancellationToken ApplicationStopped { get; }

        void StopApplication();
    }
}