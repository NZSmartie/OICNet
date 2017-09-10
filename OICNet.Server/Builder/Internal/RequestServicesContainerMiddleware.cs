// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OICNet.Server.Hosting;

namespace OICNet.Server.Builder.Internal
{
    public class RequestServicesContainerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _scopeFactory;

        public RequestServicesContainerMiddleware(RequestDelegate next, IServiceScopeFactory scopeFactory)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next)); ;
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        }

        public async Task Invoke(OicContext oicContext)
        {
            Debug.Assert(oicContext != null);

            // All done if RequestServices is set
            if (oicContext.RequestServices != null)
            {
                await _next.Invoke(oicContext);
                return;
            }

            var replacementFeature = new RequestServicesFeature(_scopeFactory);

            try
            {
                oicContext.RequestServices = replacementFeature.RequestServices;
                await _next.Invoke(oicContext);
            }
            finally
            {
                replacementFeature.Dispose();
            }
        }
    }
}