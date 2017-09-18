using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace OICNet.ResourceTypes
{
#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    [OicResourceType("oic.r.operational.state")]
    public class OperationalState : OicCoreResource
    {
        public OperationalState()
            : base(OicResourceInterface.Baseline | OicResourceInterface.Actuator, "oic.r.operational.state")
        { }

        /// <summary>
        /// Array of the possible operational states.
        /// </summary>
        [JsonProperty("machineStates", Required = Required.Always, Order = 10)]
        public List<string> MachineStates { get; set; }

        /// <summary>
        /// Current state of operation of the device.
        /// </summary>
        [JsonProperty("currentMachineState", Required = Required.Always, Order = 11)]
        public string CurrentMachineState { get; set; }

        /// <summary>
        /// Array of the possible job states.
        /// </summary>
        [JsonProperty("jobStates", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore, Order = 12)]
        public List<string> JobStates { get; set; }

        /// <summary>
        /// Currently active jobState.
        /// </summary>
        [JsonProperty("currentJobState", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore, Order = 13)]
        public string CurrentJobState { get; set; }

        /// <summary>
        /// Elapsed time in the current operational state.
        /// </summary>
        [JsonProperty("runningTime", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore, Order = 14)]
        public string RunningTime { get; set; }

        /// <summary>
        /// Time till completion of the current operational state.
        /// </summary>
        [JsonProperty("remainingTime", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore, Order = 15)]
        public string RemainingTime { get; set; }

        /// <summary>
        /// Percentage completeness of the current jobState.
        /// </summary>
        [JsonProperty("progressPercentage", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore, Order = 16)]
        public int ProgressPercentage { get; set; }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            var other = obj as OperationalState;
            if (other == null)
                return false;
            if (!base.Equals(obj))
                return false;
            if (!MachineStates.SequenceEqual(other.MachineStates))
                return false;
            if (CurrentJobState != other.CurrentMachineState)
                return false;
            if (!JobStates.SequenceEqual(other.JobStates))
                return false;
            if (CurrentJobState != other.CurrentJobState)
                return false;
            if (RunningTime != other.RunningTime)
                return false;
            if (RemainingTime != other.RemainingTime)
                return false;
            if (ProgressPercentage != other.ProgressPercentage)
                return false;
            return true;
        }
    }
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
}
