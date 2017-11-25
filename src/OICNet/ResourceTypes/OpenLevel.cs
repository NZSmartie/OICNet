using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using Newtonsoft.Json;

namespace OICNet.ResourceTypes
{
#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    [OicResourceType("oic.r.openlevel")]
    public class OpenLevel : OicCoreResource
    {
        public OpenLevel()
            : base(OicResourceInterface.Baseline | OicResourceInterface.Actuator, "oic.r.openlevel")
        { }

        /// <summary>
        /// How open or ajar the entity is.
        /// </summary>
        [JsonProperty("openLevel", Required = Required.Always, Order = 10)]
        public int OpenLevelAmount { get; set; }

        /// <summary>
        /// The step between possible values.
        /// </summary>
        [OicJsonReadOnly]
        [JsonProperty("increment", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore, Order = 11)]
        public int Increment { get; set; }

        /// <summary>
        /// Lower bound=closed, Upper bound=open.
        /// </summary>
        [JsonProperty("range", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore, Order = 12)]
        public List<int> Range { get; set; }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            var other = obj as OpenLevel;
            if (other == null)
                return false;
            if (!base.Equals(obj))
                return false;
            if (OpenLevelAmount != other.OpenLevelAmount)
                return false;
            if (Increment != other. Increment)
                return false;
            if (!Range.SequenceEqual(other.Range))
                return false;
            return true;
        }
    }
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
}
