using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace OICNet.ResourceTypes
{
    [OicResourceType("oic.r.openlevel")]
    public class OpenLevel : OicCoreResource 
    {
        /// <summary>
        /// How open or ajar the entity is.
        /// </summary>
        [JsonProperty("openLevel", Required = Required.Always, Order = 10)]
        public int OpenLevelAmount { get; set; }

        /// <summary>
        /// The step between possible values.
        /// </summary>
        [JsonProperty("increment", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore, Order = 11)]
        public int Increment { get; set; }

        /// <summary>
        /// Lower bound=closed, Upper bound=open.
        /// </summary>
        [JsonProperty("range", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore, Order = 12)]
        public List<int> Range { get; set; }
    }
}
