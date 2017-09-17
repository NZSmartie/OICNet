using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

using Newtonsoft.Json;
using OICNet.ResourceTypes;

namespace OICNet.CoreResources
{
#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    [OicResourceType("oic.wk.res")]
    public class OicResourceDirectory : OicCoreResource
    {
        // "Hack" to get around required "if" property in base-class
        public override bool ShouldSerializeInterfaces() { return false; }

        /// <summary>
        /// Unique identifier for device (UUID) as indicated by the /oic/d resource of the device
        /// </summary>
        [JsonProperty("di", Required = Required.Always, Order = 10)]
        public Guid DeviceId { get; set; }

        /// <summary>
        /// Supported messaging protocols
        /// </summary>
        [JsonProperty("mpro", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore, Order = 10), StringLength(64)]
        public string MessagingProtocols { get; set; }

        [JsonProperty("links", Required = Required.Always, Order = 11)]
        public IList<OicResourceLink> Links { get; set; } = new List<OicResourceLink>();

        public override bool Equals(object obj)
        {
            var other = obj as OicResourceDirectory;
            if (other == null)
                return false;
            if (!base.Equals(obj))
                return false;
            if (DeviceId != other.DeviceId)
                return false;
            if (MessagingProtocols != other.MessagingProtocols)
                return false;
            if (!Links.SequenceEqual(other.Links))
                return false;
            return true;
        }
    }
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
}
