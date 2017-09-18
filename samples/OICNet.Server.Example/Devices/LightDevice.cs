using System;
using OICNet.ResourceTypes;

namespace OICNet.Server.Example.Devices
{
    public class LightDevice : OicDevice
    {
        [OicResource(OicResourcePolicies.Discoverable | OicResourcePolicies.Secure)]
        public LightBrightness Brightness { get; } = new LightBrightness
        {
            RelativeUri = "/light/brightness",
            ResourceTypes = { "oic.r.light.brightness" },
            Interfaces = OicResourceInterface.Baseline | OicResourceInterface.Actuator
        };

        [OicResource(OicResourcePolicies.Discoverable | OicResourcePolicies.Secure)]
        public SwitchBinary Switch { get; } = new SwitchBinary
        {
            RelativeUri = "/light/switch",
            ResourceTypes = { "oic.r.switch.binary" },
            Interfaces = OicResourceInterface.Baseline | OicResourceInterface.Actuator
        };

        public LightDevice()
            : base("oic.d.light")
        { }
    }
}