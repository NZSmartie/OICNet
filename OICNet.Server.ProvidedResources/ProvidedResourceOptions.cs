using System;
using OICNet.Server.Builder;

namespace OICNet.Server.ProvidedResources
{
    public class ProvidedResourceOptions
    {
        public string RequestPath { get; set; }

        public IOicResourceProvider ResourceProvider { get; set; }
    }
}