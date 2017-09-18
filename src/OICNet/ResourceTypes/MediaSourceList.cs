using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace OICNet.ResourceTypes
{
#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    public class MediaSourceBase : OicCoreResource
    {
        protected MediaSourceBase()
        { }

        protected MediaSourceBase(OicResourceInterface interfaces, params string[] resourceTypes)
            : base(interfaces, resourceTypes)
        { }

        /// <summary>
        /// Specifies a pre-defined media input or output.
        /// </summary>
        [JsonProperty("sources", Required = Required.Always, Order = 10)]
        public List<MediaSource> SourceItems { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as MediaSourceBase;
            if (other == null)
                return false;
            if (!base.Equals(obj))
                return false;
            if (!SourceItems.SequenceEqual(other.SourceItems))
                return false;
            return true;
        }
    }

    [OicResourceType("oic.r.mediasourcelist")]
    public class MediaSourceList : MediaSourceBase
    {
        public MediaSourceList()
            : base(OicResourceInterface.Baseline | OicResourceInterface.Actuator, "oic.r.mediasourcelist")
        { }
    }

    [OicResourceType("oic.r.media.input")]
    public class MediaSourceInput : MediaSourceBase
    {
        public MediaSourceInput()
            : base(OicResourceInterface.Baseline | OicResourceInterface.Actuator, "oic.r.media.input")
        { }
    }

    [OicResourceType("oic.r.media.output")]
    public class MediaSourceOutput : MediaSourceBase
    {
        public MediaSourceOutput()
            : base(OicResourceInterface.Baseline | OicResourceInterface.Actuator, "oic.r.media.output")
        { }
    }
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
}
