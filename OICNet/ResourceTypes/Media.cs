using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace OICNet.ResourceTypes
{
    [OicResourceType("oic.r.media")]
    public class Media : OicCoreResource
    {
        /// <summary>
        /// The state of the door (open or closed).
        /// </summary>
        [JsonProperty("media", Required = Required.Always, Order = 10)]
        public List<MediaItem> MediaItems { get; set; }
    }

    public class MediaItem
    {
        /// <summary>
        /// URL for the media instance.
        /// </summary>
        [JsonProperty("url", Required = Required.Always, Order = 1)]
        public string Url { get; set; }
        
        /// <summary>
        /// Array of SDP media or attributes.
        /// </summary>
        [JsonProperty("sdp", Required = Required.Always, Order = 2)]
        public List<string> Sdp { get; set; }
    }
}
