using System;
using OICNet.ResourceTypes;
using System.ComponentModel;
using Microsoft.Extensions.Logging;

namespace OICNet.Server.Example.Devices
{
    public class LightDevice : OicDevice
    {
        private readonly ILogger<LightDevice> _logger;

        [OicResource(OicResourcePolicies.Discoverable | OicResourcePolicies.Secure)]
        public LightBrightness Brightness { get; } = new LightBrightness
        {
            RelativeUri = "/light/brightness",
        };

        [OicResource(OicResourcePolicies.Discoverable | OicResourcePolicies.Secure)]
        public SwitchBinary Switch { get; } = new SwitchBinary
        {
            RelativeUri = "/light/switch",
        };

        public LightDevice(ILogger<LightDevice> logger)
            : base("oic.d.light")
        {
            _logger = logger;

            Brightness.PropertyChanged += OnResourceChanged;
            Switch.PropertyChanged += OnResourceChanged;
        }

        private void OnResourceChanged(object sender, PropertyChangedEventArgs e)
        {
            if(sender == Switch && e.PropertyName == nameof(Switch.Value))
            {
                _logger.LogInformation($"Switch changed to {Switch.Value}");
            }
            else if (sender == Brightness && e.PropertyName == nameof(Brightness.Brightness))
            {
                _logger.LogInformation($"Brightness changed to {Brightness.Brightness}");
            }
        }
    }
}