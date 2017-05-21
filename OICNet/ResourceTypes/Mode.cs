using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace OICNet.ResourceTypes
{
    [OicResourceType("oic.r.mode")]
    public class Mode : OicCoreResource 
    {
        /// <summary>
        /// Array of possible modes the device supports.
        /// </summary>
        [JsonProperty("supportedModes", Required = Required.Always, Order = 10)]
        public List<string> SupportedModes { get; set; }

        /// <summary>
        /// Array of the currently active mode(s).
        /// </summary>
        [JsonProperty("modes", Required = Required.Always, Order = 11)]
        public List<string> Modes { get; set; }
    }
}
