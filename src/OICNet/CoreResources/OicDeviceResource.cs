using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using OICNet.Utilities;

namespace OICNet.CoreResources
{
    [OicResourceType("oic.wk.d")]
#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    public class OicDeviceResource : OicCoreResource
    {
        public override bool ShouldSerializeInterfaces() { return false; }

        public override bool ShouldSerializeId() { return false; }

        /// <summary>
        /// Unique identifier for device
        /// </summary>
        [JsonProperty("di"), JsonRequired]
        public Guid DeviceId { get; set; }

        /// <summary>
        /// The version of the OIC Server
        /// </summary>
        [JsonProperty("icv"), JsonRequired, MaxLength(64)]
        public string ServerVersion { get; set; }
        
        /// <summary>
        /// Spec versions of the Resource and Device Specifications to which this device data model is implemented
        /// </summary>
        [JsonProperty("dmv"), JsonRequired, MaxLength(256)]
        public string SpecVersions { get; set; }

        /// <summary>
        /// Device description in the indicated language.
        /// </summary>
        [JsonProperty("ld", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore), MinLength(1)]
        public IList<LocalisedDescription> LocalisedDescriptions { get; set; } = new List<LocalisedDescription>();

        public bool ShouldSerializeLocalisedDescriptions() { return LocalisedDescriptions.Count > 0; }

        /// <summary>
        /// Software version
        /// </summary>
        [JsonProperty("sv", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore), MaxLength(64)]
        public string SoftwareVersion { get; set; }

        /// <summary>
        /// Manufacturer name in the indicated language.
        /// </summary>
        [JsonProperty("dmn", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore), MinLength(1)]
        public IList<LocalisedDescription> ManufacturerName { get; set; } = new List<LocalisedDescription>();

        public bool ShouldSerializeManufacturerName() { return ManufacturerName.Count > 0; }

        /// <summary>
        /// Model number as designated by manufacturer
        /// </summary>
        [JsonProperty("dmno", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore), MaxLength(64)]
        public string Model { get; set; }

        /// <summary>
        /// Protocol independent unique identifier for device that is immutable
        /// </summary>
        [JsonProperty("piid"), JsonRequired]
        public Guid PlatformId { get; set; }

        public OicDeviceResource()
            : base(OicResourceInterface.Baseline | OicResourceInterface.ReadOnly, "oic.wk.d")
        {
            RelativeUri = "/oic/d";
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            var other = obj as OicDeviceResource;
            if (other == null)
                return false;
            if (DeviceId != other.DeviceId)
                return false;
            if (ServerVersion != other.ServerVersion)
                return false;
            if (SpecVersions != other.SpecVersions)
                return false;
            if (!LocalisedDescriptions.NullRespectingSequenceEqual(other.LocalisedDescriptions))
                return false;
            if (SoftwareVersion != other.SoftwareVersion)
                return false;
            if (!ManufacturerName.NullRespectingSequenceEqual(other.ManufacturerName))
                return false;
            if (Model != other.Model)
                return false;
            if (PlatformId != other.PlatformId)
                return false;
            return true;
        }

        #region Todo: Create sub-classes of the enclosed JSON schema
        //[
        //  {
        //    "devicename": "Air Conditioner",
        //    "devicetype": "oic.d.airconditioner",
        //    "resources": [
        //      {"resourcetypetitle": "Binary Switch", "resourcetypeid": "oic.r.switch.binary"},
        //      {"resourcetypetitle": "Temperature", "resourcetypeid": "oic.r.temperature"}
        //    ]
        //  },
        //  {
        //    "devicename": "Air Purifier",
        //    "devicetype": "oic.d.airpurifier",
        //    "resources": [
        //      {"resourcetypetitle": "Binary Switch", "resourcetypeid": "oic.r.switch.binary"}
        //    ]
        //  },
        //  {
        //    "devicename": "Air Quality Monitor",
        //    "devicetype": "oic.d.airqualitymonitor",
        //    "resources": [
        //      {"resourcetypetitle": "Air Quality Collection", "resourcetypeid": "oic.r.airqualitycollection"}
        //    ]
        //  },
        //  {
        //    "devicename": "Blind",
        //    "devicetype": "oic.d.blind",
        //    "resources": [
        //      {"resourcetypetitle": "Open Level", "resourcetypeid": "oic.r.openlevel"}
        //    ]
        //  },
        //  {
        //    "devicename": "Bridge",
        //    "devicetype": "oic.d.bridge",
        //    "resources": [
        //      {"resourcetypetitle": "Secure Mode", "resourcetypeid": "oic.r.securemode"}
        //    ]
        //  },
        //  {
        //    "devicename": "Camera",
        //    "devicetype": "oic.d.camera",
        //    "resources": [
        //      {"resourcetypetitle": "Media", "resourcetypeid": "oic.r.media"}
        //    ]
        //  },
        //  {
        //    "devicename": "Clothes Washer Dryer",
        //    "devicetype": "oic.d.washerdryer",
        //    "resources": [
        //      {"resourcetypetitle": "Binary Switch", "resourcetypeid": "oic.r.switch.binary"},
        //      {"resourcetypetitle": "Operational State", "resourcetypeid": "oic.r.operational.state"}
        //    ]
        //  },
        //  {
        //    "devicename": "Cooker Hood",
        //    "devicetype": "oic.d.cookerhood",
        //    "resources": [
        //      {"resourcetypetitle": "Airflow Control", "resourcetypeid": "oic.r.airflowcontrol"},
        //      {"resourcetypetitle": "Binary Switch", "resourcetypeid": "oic.r.switch.binary"},
        //      {"resourcetypetitle": "Mode", "resourcetypeid": "oic.r.mode"}
        //    ]
        //  },
        //  {
        //    "devicename": "Cooktop",
        //    "devicetype": "oic.d.cooktop",
        //    "resources": [
        //      {"resourcetypetitle": "Heating Zone Collection", "resourcetypeid": "oic.r.heatingzonecollection"}
        //    ]
        //  },
        //  {
        //    "devicename": "Dehumidifier",
        //    "devicetype": "oic.d.dehumidifier",
        //    "resources": [
        //      {"resourcetypetitle": "Binary Switch", "resourcetypeid": "oic.r.switch.binary"},
        //      {"resourcetypetitle": "Humidity", "resourcetypeid": "oic.r.humidity"}
        //    ]
        //  },
        //  {
        //    "devicename": "Dishwasher",
        //    "devicetype": "oic.d.dishwasher",
        //    "resources": [
        //      {"resourcetypetitle": "Binary Switch", "resourcetypeid": "oic.r.switch.binary"},
        //      {"resourcetypetitle": "Mode", "resourcetypeid": "oic.r.mode"}
        //    ]
        //  },
        //  {
        //    "devicename": "Door",
        //    "devicetype": "oic.d.door",
        //    "resources": [
        //      {"resourcetypetitle": "Open Level", "resourcetypeid": "oic.r.openlevel"}
        //    ]
        //  },
        //  {
        //    "devicename": "Dryer (Laundry)",
        //    "devicetype": "oic.d.dryer",
        //    "resources": [
        //      {"resourcetypetitle": "Binary Switch", "resourcetypeid": "oic.r.switch.binary"},
        //      {"resourcetypetitle": "Mode", "resourcetypeid": "oic.r.mode"}
        //    ]
        //  },
        //  {
        //    "devicename": "Fan",
        //    "devicetype": "oic.d.fan",
        //    "resources": [
        //      {"resourcetypetitle":"Binary Switch", "resourcetypeid": "oic.r.switch.binary"}
        //    ]
        //  },
        //  {
        //    "devicename": "Food Probe",
        //    "devicetype": "oic.d.foodprobe",
        //    "resources": [
        //      {"resourcetypetitle": "Temperature (Sensor)", "resourcetypeid": "oic.r.temperature"}
        //    ]
        //  },
        //  {
        //    "devicename": "Freezer",
        //    "devicetype": "oic.d.freezer",
        //    "resources": [
        //      {"resourcetypetitle": "Temperature (Sensor)", "resourcetypeid": "oic.r.temperature"},
        //      {"resourcetypetitle": "Temperature (Actuator)", "resourcetypeid": "oic.r.temperature"}
        //    ]
        //  },
        //  {
        //    "devicename": "Garage Door",
        //    "devicetype": "oic.d.garagedoor",
        //    "resources": [
        //      {"resourcetypetitle": "Door", "resourcetypeid": "oic.r.door"}
        //    ]
        //  },
        //  {
        //    "devicename": "Generic Sensor",
        //    "devicetype": "oic.d.sensor",
        //    "resources": [
        //      {"resourcetypetitle": "Any Resource Type that supports and exposes in “/oic/res” the oic.if.s interface.",
        //       "resourcetypeid": "oic.r.<x>"}
        //     ]
        //   },
        //   {
        //     "devicename": "Humidifier",
        //     "devicetype": "oic.d.humidifier",
        //     "resources": [
        //       {"resourcetypetitle":"Binary Switch", "resourcetypeid": "oic.r.switch.binary"}
        //     ]
        //   },
        //   {
        //     "devicename": "Light",
        //     "devicetype": "oic.d.light",
        //     "resources": [
        //       {"resourcetypetitle": "Binary Switch", "resourcetypeid": "oic.r.switch.binary"}
        //     ]
        //   },
        //   {
        //     "devicename": "Oven",
        //     "devicetype": "oic.d.oven",
        //     "resources": [
        //       {"resourcetypetitle": "Binary Switch", "resourcetypeid": "oic.r.switch.binary"},
        //       {"resourcetypetitle": "Temperature (Sensor)", "resourcetypeid": "oic.r.temperature"},
        //       {"resourcetypetitle": "Temperature (Actuator)", "resourcetypeid": "oic.r.temperature"}
        //     ]
        //   },
        //   {
        //     "devicename": "Printer",
        //     "devicetype": "oic.d.printer",
        //     "resources": [
        //       {"resourcetypetitle": "Binary Switch", "resourcetypeid": "oic.r.switch.binary"},
        //       {"resourcetypetitle": "Operational State", "resourcetypeid": "oic.r.operational.state"}
        //     ]
        //   },
        //   {
        //     "devicename": "Printer Multi-Function",
        //     "devicetype": "oic.d.multifunctionprinter",
        //     "resources": [
        //       {"resourcetypetitle": "Binary switch", "resourcetypeid": "oic.r.switch.binary"},
        //       {"resourcetypetitle": "Operational State (Printer)", "resourcetypeid": "oic.r.operational.state"},
        //       {"resourcetypetitle": "Operational State (Scanner)", "resourcetypeid": "oic.r.operational.state"},
        //       {"resourcetypetitle": "Automatic Document Feeder", "resourcetypeid": "oic.r.automaticdocumentfeeder"}
        //     ]
        //   },
        //   {
        //     "devicename": "Receiver",
        //     "devicetype": "oic.d.receiver",
        //     "resources": [
        //       {"resourcetypetitle": "Binary Switch", "resourcetypeid": "oic.r.switch.binary"},
        //       {"resourcetypetitle": "Audio Controls", "resourcetypeid": "oic.r.audio"},
        //       {"resourcetypetitle": "Media Source List", "resourcetypeid": "oic.r.media.input"},
        //       {"resourcetypetitle": "Media Source List", "resourcetypeid": "oic.r.media.output"}

        //     ]
        //   },
        //   {
        //     "devicename": "Refrigerator",
        //     "devicetype": "oic.d.refrigerator",
        //     "resources": [
        //       {"resourcetypetitle": "Temperature (Sensor)", "resourcetypeid": "oic.r.temperature"},
        //       {"resourcetypetitle": "Temperature (Actuator)", "resourcetypeid": "oic.r.temperature"}
        //     ]
        //   },
        //   {
        //     "devicename": "Robot Cleaner",
        //     "devicetype": "oic.d.robotcleaner",
        //     "resources": [
        //       {"resourcetypetitle": "Binary Switch", "resourcetypeid": "oic.r.switch.binary"},
        //       {"resourcetypetitle": "Mode", "resourcetypeid": "oic.r.mode"}
        //     ]
        //   },
        //   {
        //     "devicename": "Scanner",
        //     "devicetype": "oic.d.scanner",
        //     "resources": [
        //       {"resourcetypetitle": "Binary switch", "resourcetypeid": "oic.r.switch.binary"},
        //       {"resourcetypetitle": "Operational State", "resourcetypeid": "oic.r.operational.state"},
        //       {"resourcetypetitle": "Automatic Document Feeder", "resourcetypeid": "oic.r.automaticdocumentfeeder"}
        //     ]
        //   },
        //   {
        //     "devicename": "Security Panel",
        //     "devicetype": "oic.d.securitypanel",
        //     "resources": [
        //       {"resourcetypetitle": "Mode", "resourcetypeid": "oic.r.mode"}
        //     ]
        //   },
        //   {
        //     "devicename": "Set Top Box",
        //     "devicetype": "oic.d.stb",
        //     "resources": [
        //       {"resourcetypetitle":"Binary Switch", "resourcetypeid": "oic.r.switch.binary"}
        //     ]
        //   },
        //   {
        //     "devicename": "Smart Lock",
        //     "devicetype": "oic.d.smartlock",
        //     "resources": [
        //       {"resourcetypetitle": "Lock Status", "resourcetypeid": "oic.r.lock.status"}
        //     ]
        //   },
        //   {
        //     "devicename": "Smart Plug",
        //     "devicetype": "oic.d.smartplug",
        //     "resources": [
        //       {"resourcetypetitle": "Binary Switch", "resourcetypeid": "oic.r.switch.binary"}
        //     ]
        //   },
        //   {
        //     "devicename": "Switch",
        //     "devicetype": "oic.d.switch",
        //     "resources": [
        //       {"resourcetypetitle": "Binary Switch", "resourcetypeid": "oic.r.switch.binary"}
        //     ]
        //   },
        //   {
        //     "devicename": "Television",
        //     "devicetype": "oic.d.tv",
        //     "resources": [
        //       {"resourcetypetitle": "Binary Switch", "resourcetypeid": "oic.r.switch.binary"},
        //       {"resourcetypetitle": "Audio Controls", "resourcetypeid": "oic.r.audio"},
        //       {"resourcetypetitle": "Media Source List", "resourcetypeid": "oic.r.media.input"}
        //     ]
        //   },
        //   {
        //     "devicename": "Thermostat",
        //     "devicetype": "oic.d.thermostat",
        //     "resources": [
        //       {"resourcetypetitle": "Temperature (Sensor)", "resourcetypeid": "oic.r.temperature"},
        //       {"resourcetypetitle": "Temperature (Actuator)", "resourcetypeid": "oic.r.temperature"}
        //     ]
        //   },
        //   {
        //     "devicename": "Washer (Laundry)",
        //     "devicetype": "oic.d.washer",
        //     "resources": [
        //       {"resourcetypetitle": "Binary Switch", "resourcetypeid": "oic.r.switch.binary"},
        //       {"resourcetypetitle": "Operational State", "resourcetypeid": "oic.r.operational.state"}
        //     ]
        //   },
        //   {
        //     "devicename": "Water Valve",
        //     "devicetype": "oic.d.watervalve",
        //     "resources": [
        //       {"resourcetypetitle": "Open Level", "resourcetypeid": "oic.r.openlevel"}
        //     ]
        //   }
        //]

        #endregion
    }
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()

    public class LocalisedDescription
    {
        // Todo: "oic.types-schema.json#/definitions/language-tag"
        /// <summary>
        /// An RFC 5646 language tag.
        /// </summary>
        [JsonProperty("language")]
        public CultureInfo Culture { get; set; }

        /// <summary>
        /// Localised Description
        /// </summary>
        [JsonProperty("value"), MaxLength(64)]
        public string Description { get; set; }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            var other = obj as LocalisedDescription;
            if (other == null)
                return false;
            if (Culture != other.Culture)
                return false;
            if (Description!= other.Description)
                return false;
            return true;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return typeof(LocalisedDescription).GetHashCode();
        }
    }
}
