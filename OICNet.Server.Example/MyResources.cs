using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OICNet.Server.ProvidedResources;

namespace OICNet.Server.Example
{
    public class MyResources : IOicResourceProvider
    {
        private readonly IOicResource _helloResource;

        public MyResources()
        {
            _helloResource = new OicBaseResouece<string> { Value = "Hello World"};
        }

        public IOicResource GetResource(string id)
        {
            if (string.Equals(id, "/hello", StringComparison.OrdinalIgnoreCase))
                return _helloResource;
            return null;
        }
    }
}