using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace OICNet.ResourceTypes
{
    public enum MediaSourceType
    {
        [EnumMember(Value = "audioOnly")]
        AudioOnly,
        [EnumMember(Value = "videoOnly")]
        VideoOnly,
        [EnumMember(Value = "audioPlusVideo")]
        AudioPlusVideo
    }

#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    [OicResourceType("oic.r.mediasource")]
    public class MediaSource : OicCoreResource
    {
        /// <summary>
        /// Specifies a pre-defined media input or output.
        /// </summary>
        [JsonProperty("sourceName", Required = Required.Always, Order = 10)]
        public string OpenDuration { get; set; }


        /// <summary>
        /// Numeric identifier to specify the instance.
        /// </summary>
        [JsonProperty("sourceNumber", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore, Order = 11)]
        public int SourceNumber { get; set; }

        /// <summary>
        /// Specifies the type of the source.
        /// </summary>
        [JsonProperty("sourceType", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore, Order = 12, ItemConverterType = typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public MediaSourceType SourceType { get; set; }

        /// <summary>
        /// Specifies if the specific source instance is selected or not
        /// </summary>
        [JsonProperty("status", Order = 13, Required = Required.Always)]
        public bool Status { get; set; }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            var other = obj as MediaSource;
            if (other == null)
                return false;
            if (!base.Equals(obj))
                return false;
            if (OpenDuration != other.OpenDuration)
                return false;
            if (SourceNumber != other.SourceNumber)
                return false;
            if (SourceType != other.SourceType)
                return false;
            if (Status != other.Status)
                return false;
            return true;
        }
    }
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
}
