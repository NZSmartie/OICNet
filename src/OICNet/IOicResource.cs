using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace OICNet
{
    public interface IOicSerialisableResource { }

    public interface IOicResource : IOicSerialisableResource
    {
        [JsonIgnore]
        string RelativeUri { get; set; }

        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        string Id { get; set; }

        [JsonProperty("if")]
        OicResourceInterface Interfaces { get; set; }

        [JsonProperty("n", NullValueHandling = NullValueHandling.Ignore)]
        string Name { get; set; }

        [JsonProperty("rt"), JsonRequired()]
        [MinLength(1), StringLength(64)]
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