using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace OICNet.ResourceTypes
{
#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    [OicResourceType("oic.r.media")]
    public class Media : OicCoreResource
    {
        /// <summary>
        /// The state of the door (open or closed).
        /// </summary>
        [JsonProperty("media", Required = Required.Always, Order = 10)]
        public List<MediaItem> MediaItems { get; set; }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            var other = obj as Media;
            if (other == null)
                return false;
            if (!base.Equals(obj))
                return false;
            if (!MediaItems.SequenceEqual(other.MediaItems))
                return false;
            return true;
        }
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

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            var other = obj as MediaItem;
            if (other == null)
                return false;
            if (Url != other.Url)
                return false;
            if (!Sdp.SequenceEqual(other.Sdp))
                return false;
            return true;
        }
    }
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
}
