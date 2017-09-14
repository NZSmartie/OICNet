using System;
using System.IO;

namespace OICNet.Server.Hosting.Internal
{
    public static class HostingEnvironmentExtensions
    {
        public static void Initialize(this IHostingEnvironment hostingEnvironment, OicHostOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            if (string.IsNullOrEmpty(options.ApplicationName))
            {
                throw new ArgumentException("A valid non-empty application name must be provided.", nameof(options));
            }

            hostingEnvironment.ApplicationName = options.ApplicationName;

            hostingEnvironment.EnvironmentName =
                options.Environment ??
                hostingEnvironment.EnvironmentName;
        }
    }
}
