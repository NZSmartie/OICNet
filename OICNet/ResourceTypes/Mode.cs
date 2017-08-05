using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace OICNet.ResourceTypes
{
#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    [OicResourceType("oic.r.mode")]
    public class Mode : OicCoreResource
    {
        /// <summary>
        /// Array of possible modes the device supports.
        /// </summary>
        [JsonProperty("supportedModes", Required = Required.Always, Order = 10)]
        public List<string> SupportedModes { get; set; }

        /// <summary>
        /// Array of the currently active mode(s).
        /// </summary>
        [JsonProperty("modes", Required = Required.Always, Order = 11)]
        public List<string> Modes { get; set; }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            var other = obj as Mode;
            if (other == null)
                return false;
            if (!base.Equals(obj))
                return false;
            if (!SupportedModes.SequenceEqual(other.SupportedModes))
                return false;
            if (!Modes.SequenceEqual(other.Modes))
                return false;
            return true;
        }
    }
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
}
