using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

    public class OicDevice : INotifyPropertyChanged
    {
        public IReadOnlyList<string> DeviceTypes { get; }

        private string _name;

        public virtual string Name
        {
            get => _name;
            set
            {
                if (value == _name)
                    return;
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        private Guid _deviceId;
        public virtual Guid DeviceId
        {
            get => _deviceId; set
            {
                if (_deviceId == value)
                    return;
                _deviceId = value;
                OnPropertyChanged(nameof(DeviceId));
            }
        }

        // TODO: Do we want to remove this in favour of finding resources in subclasses? or make this a lambda?
        public virtual ObservableCollection<IOicResource> Resources { get; } = new ObservableCollection<IOicResource>();

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

        // TODO: Does this need to be internal? would public access be benificial?
        internal void UpdateResourceInternal(IOicResource resource)
        {
            var device = resource as CoreResources.OicDeviceResource;
            if (device == null)
                return;

            Name = device.Name;
            DeviceId = device.DeviceId;

            // TODO: auto map OicDeviceResource to OicDevice? or make it a field/property?
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class OicRemoteDevice : OicDevice
    {
        public OicDevice Device { get; }
        public IOicEndpoint Endpoint { get; }

        public override string Name { get => base.Name; set => base.Name = value; }

        public override Guid DeviceId { get => base.DeviceId; set => base.DeviceId = value; }

        public override ObservableCollection<IOicResource> Resources => base.Resources;

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
