using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

namespace OICNet.ResourceTypes
{
#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    [OicResourceType("oic.r.audio")]
    public class Audio : OicCoreResource
    {
        /// <summary>
        /// Volume setting of an audio rendering device.
        /// </summary>
        [JsonProperty("volume", Required = Required.Always, Order = 10)]
        public int Volume { get; set; }

        //"minimum": 0,
        //"maximum": 100

        /// <summary>
        /// Mute setting of an audio rendering device.
        /// </summary>
        [JsonProperty("mute", Required = Required.Always, Order = 11)]
        public bool Mute { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as Audio;
            if (other == null)
                return false;
            if (!base.Equals(obj))
                return false;
            if (Volume != other.Volume)
                return false;
            if (Mute != other.Mute)
                return false;
            return true;
        }
    }
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
}
