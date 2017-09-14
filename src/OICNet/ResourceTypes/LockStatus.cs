using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace OICNet.ResourceTypes
{
    public enum LockState
    {
        [EnumMember(Value = "Locked")]
        Locked,
        [EnumMember(Value = "Unlocked")]
        Unlocked
    }

#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    [OicResourceType("oic.r.lock.status")]
    public class LockStatus : OicCoreResource
    {
        /// <summary>
        /// State of the lock.
        /// </summary>
        [JsonProperty("lockState", Required = Required.Always, Order = 10, ItemConverterType = typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public LockState LockState { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as LockStatus;
            if (other == null)
                return false;
            if (!base.Equals(obj))
                return false;
            if (LockState != other.LockState)
                return false;
            return true;
        }
    }
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
}
