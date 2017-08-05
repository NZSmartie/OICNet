using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace OICNet.ResourceTypes
{
    public enum TemperatureUnit
    {
        [EnumMember(Value = "C")]
        Celsius,
        [EnumMember(Value = "F")]
        Fahrenheit,
        [EnumMember(Value = "K")]
        Kelvin,
    }

#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    [OicResourceType("oic.r.temperature")]
    public class Temperature : OicCoreResource
    {
        /// <summary>
        /// Current temperature setting or measurement.
        /// </summary>
        [JsonProperty("temperature", Required = Required.Always, Order = 10)]
        public float Value { get; set; }

        /// <summary>
        /// Units for the temperature value.
        /// </summary>
        [JsonProperty("units", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore, ItemConverterType = typeof(Newtonsoft.Json.Converters.StringEnumConverter), Order = 11)]
        public TemperatureUnit Units { get; set; }

        /// <summary>
        /// Array defining min,max values for this temperature on this device.
        /// </summary>
        [JsonProperty("range", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore, Order = 12)]
        public List<float> Range { get; set; }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            var other = obj as Temperature;
            if (other == null)
                return false;
            if (!base.Equals(obj))
                return false;
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (Value != other.Value)
                return false;
            if (Units != other.Units)
                return false;
            if (!Range.SequenceEqual(other.Range))
                return false;
            return true;
        }
    }
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
}
