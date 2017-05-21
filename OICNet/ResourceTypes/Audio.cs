using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

namespace OICNet.ResourceTypes
{
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
    }
}
