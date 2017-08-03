using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace OICNet.ResourceTypes
{
    public enum DoorOpenState
    {
        [EnumMember(Value = "Open")]
        Open,
        [EnumMember(Value = "Closed")]
        Closed
    }

    [OicResourceType("oic.r.door")]
    public class Door : OicCoreResource
    {
        /// <summary>
        /// The state of the door (open or closed).
        /// </summary>
        [JsonProperty("openState", Required = Required.Always, Order = 10, ItemConverterType = typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public DoorOpenState OpenState { get; set; }

        /// <summary>
        /// The time duration the door has been open.
        /// </summary>
        [JsonProperty("openDuration", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore, Order = 11)]
        public string OpenDuration { get; set; }

        /// <summary>
        /// The state of the door open alarm.
        /// </summary>
        [JsonProperty("openAlarm", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore, Order = 12)]
        public bool OpenAlarm { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as Door;
            if (other == null)
                return false;
            if (!base.Equals(obj))
                return false;
            if (OpenState!= other.OpenState)
                return false;
            if (OpenDuration!= other.OpenDuration)
                return false;
            if (OpenAlarm!= other.OpenAlarm)
                return false;
            return true;
        }
    }
}
