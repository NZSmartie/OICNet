using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OICNet
{
    public interface IOicSerialisableResource { }

    public interface IOicResource : IOicSerialisableResource
    {
        string RelativeUri { get; set; }

        string Id { get; set; }

        OicResourceInterface Interfaces { get; set; }

        string Name { get; set; }

        IList<string> ResourceTypes { get; set; }

        void UpdateFields(IOicResource source);
    }

    public class OicResourceList : List<IOicResource>, IOicSerialisableResource
    {
        public OicResourceList()
            :base()
        { }

        public OicResourceList(IEnumerable<IOicResource> collection)
            : base(collection)
        { }
        
        public OicResourceList(int capacity)
            : base(capacity)
        { }
    }
}