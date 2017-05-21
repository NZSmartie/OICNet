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

    [OicResourceType("oic.r.lock.status")]
    public class LockStatus : OicCoreResource
    {
        /// <summary>
        /// State of the lock.
        /// </summary>
        [JsonProperty("lockState", Required = Required.Always, Order = 10, ItemConverterType = typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public LockState LockState { get; set; }
    }
}
