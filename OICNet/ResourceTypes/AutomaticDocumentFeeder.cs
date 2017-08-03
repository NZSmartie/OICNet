using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

namespace OICNet.ResourceTypes
{
    [OicResourceType("oic.r.automaticdocumentfeeder")]
    public class AutomaticDocumentFeeder : OicCoreResource
    {
        /// <summary>
        /// Array of the possible adf states.
        /// </summary>
        [JsonProperty("adfStates", Required = Required.Always, Order = 10)]
        public List<string> AdfStates { get; set; }

        /// <summary>
        /// Current adf state.
        /// </summary>
        [JsonProperty("currentAdfState", Required = Required.Always, Order = 11)]
        public string CurrentAdfState { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as AutomaticDocumentFeeder;
            if (other == null)
                return false;
            if (!base.Equals(obj))
                return false;
            if (AdfStates!= other.AdfStates)
                return false;
            if (CurrentAdfState!= other.CurrentAdfState)
                return false;
            return true;
        }
    }
}
