using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OICNet.CoreResources
{
    [OicResourceType("oic.wk.p")]
    public class OicPlatform : OicCoreResource
    {
        public override bool ShouldSerializeInterfaces() { return false; }

        public override bool ShouldSerializeId() { return false; }

        public override bool ShouldSerializeName() { return false; }

        /// <summary>
        /// Platform Identifier
        /// </summary>
        [JsonProperty("pi"), JsonRequired]
        public Guid PlatformId { get; set; }

        /// <summary>
        /// Manufacturer Name
        /// </summary>
        [JsonProperty("mnmn"), JsonRequired, MaxLength(64)]
        public string ManufacturerName { get; set; }

        /// <summary>
        /// Manufacturer's URL
        /// </summary>
        [JsonProperty("mnml", NullValueHandling = NullValueHandling.Ignore, Required = Required.DisallowNull), MaxLength(256)]
        public Uri ManufacturerUrl { get; set; }

        /// <summary>
        /// Model number as designated by the manufacturer
        /// </summary>
        [JsonProperty("mnmo", NullValueHandling = NullValueHandling.Ignore, Required = Required.DisallowNull), MaxLength(64)]
        public string ModelNumber { get; set; }

        /// <summary>
        /// Manufacturing Date in ISO8601 format.
        /// </summary>
        [JsonProperty("mndt", NullValueHandling = NullValueHandling.Ignore, Required = Required.DisallowNull)]
        public DateTime ManufacturingDate { get; set; }

        /// <summary>
        /// Platform Version
        /// </summary>
        [JsonProperty("mnpv", NullValueHandling = NullValueHandling.Ignore, Required = Required.DisallowNull), MaxLength(64)]
        public string PlatformVersion { get; set; }

        /// <summary>
        /// Platform Resident OS Version
        /// </summary>
        [JsonProperty("mnos", NullValueHandling = NullValueHandling.Ignore, Required = Required.DisallowNull), MaxLength(64)]
        public string PlatformOperatingSystemVersion { get; set; }

        /// <summary>
        /// Platform Hardware Version
        /// </summary>
        [JsonProperty("mnhw", NullValueHandling = NullValueHandling.Ignore, Required = Required.DisallowNull), MaxLength(64)]
        public string PlatformHardwareVersion { get; set; }

        /// <summary>
        /// Manufacturer's firmware version
        /// </summary>
        [JsonProperty("mnfv", NullValueHandling = NullValueHandling.Ignore, Required = Required.DisallowNull), MaxLength(64)]
        public string FirmwareVersion { get; set; }

        /// <summary>
        /// Manufacturer's Support Information URL
        /// </summary>
        [JsonProperty("mnsl", NullValueHandling = NullValueHandling.Ignore, Required = Required.DisallowNull), MaxLength(256)]
        public Uri SupportURL { get; set; }

        /// <summary>
        /// Reference time for the device in ISO8601 format.
        /// </summary>
        [JsonProperty("st", NullValueHandling = NullValueHandling.Ignore, Required = Required.DisallowNull)]
        public DateTime CurrentTime { get; set; }

        /// <summary>
        /// Manufacturer's defined information for the platform. The content is freeform, with population rules up to the manufacturer
        /// </summary>
        [JsonProperty("vid", NullValueHandling = NullValueHandling.Ignore, Required = Required.DisallowNull), MaxLength(64)]
        public string VendorId { get; set; }
    }
}
