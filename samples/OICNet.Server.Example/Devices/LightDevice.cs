using System;
using OICNet.CoreResources;
using OICNet.ResourceTypes;
using System.Reflection;
using System.Diagnostics;

namespace OICNet.Server.Example.Devices
{
    public class LightDevice : OicDevice
    {
        [ResourceDiscoverable, ResourceSecure]
        public LightBrightness Brightness { get; } = new LightBrightness
        {
            RelativeUri = "/light/brightness",
            ResourceTypes = { "oic.r.light.brightness" },
            Interfaces = { OicResourceInterface.Baseline, OicResourceInterface.Actuator }
        };

        [ResourceDiscoverable, ResourceSecure]
        public SwitchBinary Switch { get; } = new SwitchBinary
        {
            RelativeUri = "/light/switch",
            ResourceTypes = { "oic.r.switch.binary" },
            Interfaces = { OicResourceInterface.Baseline, OicResourceInterface.Actuator }
        };

        public LightDevice()
            : base("oic.d.light")
        { }
    }
}