using System;
using System.Linq;
using System.Reflection;
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

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class OicResourceTypeAttribute : Attribute
    {
        public readonly string Id;
        
        public OicResourceTypeAttribute(string id)
        {
            Id = id;
        }
    }

    public static class OicResourceExtensions
    {
        public static string GetResourceTypeId(this OicCoreResource resource)
        {
            var info = resource.GetType()
                .GetTypeInfo()
                .GetCustomAttributes()
                .FirstOrDefault(i => i is OicResourceTypeAttribute)
                as OicResourceTypeAttribute;
            return info.Id;
        }
    }

    public class OicCoreResource : IOicResource
    {
        [JsonIgnore]
        public string RelativeUri { get; set; }

        [JsonProperty("rt"), JsonRequired()]
        [MinLength(1), StringLength(64)]
        public List<string> ResourceTypes { get; set; }

        [JsonProperty("if", ItemConverterType = typeof(Newtonsoft.Json.Converters.StringEnumConverter)), JsonRequired()]
        public List<OicResourceInterface> Interfaces { get; set; }

        [JsonProperty("n", NullValueHandling=NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as OicCoreResource;

            if (other == null)
                return false;

            if (!string.Equals(RelativeUri, other.RelativeUri))
                return false;
            if (!ResourceTypes?.SequenceEqual(other.ResourceTypes) ?? false)
                return false;
            if (!Interfaces?.SequenceEqual(other.Interfaces) ?? false)
                return false;
            if (!string.Equals(Name, other.Name))
                return false;
            if (!string.Equals(Id, other.Id))
                return false;
            return true;
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }

    public class OicBaseResouece<VType> : OicCoreResource
    {
        [JsonProperty("value", Required = Required.Always, Order = 5)]
        public VType Value { get; set; }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            var other = obj as OicBaseResouece<VType>;
            if (other == null)
                return false;
            if (!base.Equals(obj))
                return false;
            if (!Value.Equals(other.Value))
                return false;
            return true;
        }
    }

    public class OicIntResouece : OicBaseResouece<int>
    {
        [JsonProperty("range", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore, Order = 6)]
        public List<int> Range { get; set; }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            var other = obj as OicIntResouece;
            if (other == null)
                return false;
            if (!base.Equals(obj))
                return false;
            if (!Range.SequenceEqual(other.Range))
                return false;
            return true;
        }
    }

    public class OicNumberResouece : OicBaseResouece<float>
    {
        [JsonProperty("range", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore, Order = 6)]
        public List<float> Range { get; set; }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            var other = obj as OicNumberResouece;
            if (other == null)
                return false;
            if (!base.Equals(obj))
                return false;
            if (!Range.SequenceEqual(other.Range))
                return false;
            return true;
        }
    }
}
