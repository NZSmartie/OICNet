using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OICNet
{
    public class OicDeviceReceivedMessageEventArgs : EventArgs
    {
        public OicDevice Device;

        public OicMessage Message;
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class OicDeviceTypeAttribute : Attribute
    {
        public OicDeviceTypeAttribute(string type)
        {
            Type = type;
        }

        public string Type { get; }
    }

    public class OicDevice
    {
        public IReadOnlyList<string> DeviceTypes { get; }

        public virtual string Name { get; set; }

        public virtual Guid DeviceId { get; set; }

        // TODO: Do we want to remove this in favour of finding resources in subclasses? or make this a lambda?
        public virtual List<IOicResource> Resources { get; } = new List<IOicResource>();

        public OicDevice(Guid deviceId, params string[] deviceTypes)
        {
            DeviceId = deviceId;
            DeviceTypes = deviceTypes != null
                ? deviceTypes.Concat(new[] { "oic.wk.d" }).ToList()
                : new List<string> { "oic.wk.d" };
        }

        public OicDevice(params string[] deviceTypes)
            : this(Guid.NewGuid(), deviceTypes)
        { }

        internal void UpdateResourceInternal(IOicResource resource)
        {
            switch (resource)
            {
                    
            }
        }
    }

    public class OicRemoteDevice : OicDevice
    {
        public OicDevice Device { get; }
        public IOicEndpoint Endpoint { get; }

        public override string Name { get => base.Name; set => base.Name = value; }
        public override Guid DeviceId { get => base.DeviceId; set => base.DeviceId = value; }
        public override List<IOicResource> Resources => base.Resources;

        public OicRemoteDevice(IOicEndpoint endpoint)
            : this(endpoint, new OicDevice())
        { }

        public OicRemoteDevice(IOicEndpoint endpoint, OicDevice device)
        {
            Endpoint = endpoint;
            Device = device;
        }
    }
}
