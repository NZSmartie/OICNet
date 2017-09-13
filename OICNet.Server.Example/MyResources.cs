using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using OICNet.Server.ProvidedResources;
using System.Linq;

namespace OICNet.Server.Example
{
    public class MyResources : IOicResourceProvider
    {
        private readonly IOicResource _helloResource;

        public MyResources()
        {
            Resources = new ObservableCollection<IOicResource>
            {
                new OicBaseResouece<string>
                {
                    Interfaces = {OicResourceInterface.Baseline, OicResourceInterface.ReadOnly},
                    ResourceTypes = {"oicnet.hello"},
                    RelativeUri = "/hello",
                    Value = "Hello World"
                }
            };
        }

        public IList<IOicResource> Resources { get; }

        public IOicResource GetResource(Uri uri)
        {
            return Resources.FirstOrDefault(r => uri.AbsolutePath.Equals(r.RelativeUri, StringComparison.Ordinal));
        }
    }
}