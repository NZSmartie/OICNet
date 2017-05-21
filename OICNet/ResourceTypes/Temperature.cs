using System;
using System.Collections.Generic;
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
    }
}
