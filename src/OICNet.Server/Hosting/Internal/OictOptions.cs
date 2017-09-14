// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Extensions.Configuration;

namespace OICNet.Server.Hosting.Internal
{
    public class OicHostOptions
    {
        public OicHostOptions() { }

        public OicHostOptions(IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            ApplicationName = configuration[OicHostDefaults.ApplicationKey];
            StartupAssembly = configuration[OicHostDefaults.StartupAssemblyKey];
            Environment = configuration[OicHostDefaults.EnvironmentKey];
            WebRoot = configuration[OicHostDefaults.WebRootKey];
            // Search the primary assembly and configured assemblies.
            HostingStartupAssemblies = $"{ApplicationName};{configuration[OicHostDefaults.HostingStartupAssembliesKey]}"
                .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries) ?? new string[0];

            var timeout = configuration[OicHostDefaults.ShutdownTimeoutKey];
            if (!string.IsNullOrEmpty(timeout)
                && int.TryParse(timeout, NumberStyles.None, CultureInfo.InvariantCulture, out var seconds))
            {
                ShutdownTimeout = TimeSpan.FromSeconds(seconds);
            }
        }

        public string ApplicationName { get; set; }

        public IReadOnlyList<string> HostingStartupAssemblies { get; set; }

        public string Environment { get; set; }

        public string StartupAssembly { get; set; }

        public string WebRoot { get; set; }

        public TimeSpan ShutdownTimeout { get; set; } = TimeSpan.FromSeconds(5);

    }
}