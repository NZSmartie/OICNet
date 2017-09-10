// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace OICNet.Server.Builder.Internal
{
    public class AutoRequestServicesStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return builder =>
            {
                builder.UseMiddleware<RequestServicesContainerMiddleware>();
                next(builder);
            };
        }
    }
}
