using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using Newtonsoft.Json;

namespace OICNet.ResourceTypes
{
#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    [OicResourceType("oic.r.light.brightness")]
    public class LightBrightness : OicCoreResource
    {
        public LightBrightness()
            : base(OicResourceInterface.Baseline | OicResourceInterface.Actuator, "oic.r.light.brightness")
        { }

        private int _brightness;
        /// <summary>
        /// Quantized representation in the range 0-100 of the current sensed or set value for Brightness
        /// </summary>
        [JsonProperty("brightness", Required = Required.Always, Order = 10), Range(0, 100)]
        public int Brightness
        {
            get => _brightness;
            set
            {
                if (_brightness == value)
                    return;
                _brightness = value;
                OnPropertyChanged(nameof(Brightness));
            }
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            var other = obj as LightBrightness;
            if (other == null)
                return false;
            if (!base.Equals(obj))
                return false;
            if (Brightness != other.Brightness)
                return false;
            return true;
        }

        public override void UpdateFields(IOicResource source)
        {
            base.UpdateFields(source);

            if (!(source is LightBrightness brightness))
                return;

            Brightness = brightness.Brightness;
        }
    }
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
}
