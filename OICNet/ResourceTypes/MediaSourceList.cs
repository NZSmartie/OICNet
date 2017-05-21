using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace OICNet.ResourceTypes
{
    [OicResourceType("oic.r.mediasourcelist"), OicResourceType("oic.r.media.input"), OicResourceType("oic.r.media.output")]
    public class MediaSourceList : OicCoreResource 
    {
        /// <summary>
        /// Specifies a pre-defined media input or output.
        /// </summary>
        [JsonProperty("sources", Required = Required.Always, Order = 10)]
        public List<MediaSource> SourceItems { get; set; }
    }
}
