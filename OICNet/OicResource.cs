using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

using Newtonsoft.Json;

namespace OICNet
{
    public enum OicResourceInterface
    {
        [EnumMember(Value = "oic.if.baseline")]
        Baseline,
        [EnumMember(Value = "oic.if.ll")]
        LinkLists,
        [EnumMember(Value = "oic.if.b")]
        Batch,
        [EnumMember(Value = "oic.if.r")]
        ReadOnly,
        [EnumMember(Value = "oic.if.rw")]
        ReadWrite,
        [EnumMember(Value = "oic.if.a")]
        Actuator,
        [EnumMember(Value = "oic.if.s")]
        Sensor,
    }

    public class OicCoreResource
    {
        [JsonProperty("rt"), JsonRequired()]
        [MinLength(1), StringLength(64)]
        public List<string> ResourceTypes { get; set; }

        [JsonProperty("if", ItemConverterType = typeof(Newtonsoft.Json.Converters.StringEnumConverter)), JsonRequired()]
        public List<OicResourceInterface> Interfaces { get; set; }

        [JsonProperty("n", NullValueHandling=NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }
    }

    public class OicBaseResouece<VType> : OicCoreResource
    {
        [JsonProperty("value", Order = 5)]
        public VType Value { get; set; }
    }

    public class OicIntResouece : OicBaseResouece<int>
    {
        [JsonProperty("range", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore, Order = 6)]
        public List<int> Range { get; set; }
    }

    public class OicNumberResouece : OicBaseResouece<float>
    {
        [JsonProperty("range", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore, Order = 6)]
        public List<float> Range { get; set; }
    }
}
