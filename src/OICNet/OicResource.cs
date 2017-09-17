﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OICNet.Utilities;

namespace OICNet
{
    public enum OicResourceInterface
    {
        [EnumMember(Value = "oic.if.baseline")]
        Baseline,
        [EnumMember(Value = "oic.if.ll")]
        LinkLists,
        [EnumMember(Value = "oic.if.b")]
        Batch,
        [EnumMember(Value = "oic.if.r")]
        ReadOnly,
        [EnumMember(Value = "oic.if.rw")]
        ReadWrite,
        [EnumMember(Value = "oic.if.a")]
        Actuator,
        [EnumMember(Value = "oic.if.s")]
        Sensor,
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class OicResourceTypeAttribute : Attribute
    {
        public readonly string Id;
        
        public OicResourceTypeAttribute(string id)
        {
            Id = id;
        }
    }

    public static class OicResourceExtensions
    {
        public static string GetResourceTypeId(this IOicResource resource)
        {
            var info = resource.GetType()
                .GetTypeInfo()
                .GetCustomAttributes()
                .FirstOrDefault(i => i is OicResourceTypeAttribute)
                as OicResourceTypeAttribute;
            return info.Id;
        }

        public static Uri GetResourceUri(this IOicResourceRepository resource)
        {
            throw new NotImplementedException();
            //return new UriBuilder($"oic://{resource.Device.Endpoint.Authority}")
            //{
            //    Path = resource.RelativeUri
            //}.Uri;

        }
    }

    public class OicCoreResource : IOicResource, INotifyPropertyChanged
    {
        public virtual void UpdateFields(IOicResource source)
        {
            var coreResource = source as OicCoreResource ??
                               throw ExceptionUtil.CreateResourceCastException<OicCoreResource>(nameof(source));

            Name = coreResource.Name ?? Name;
            Id = coreResource.Id ?? Id;
        }

        [JsonIgnore]
        public string RelativeUri { get; set; }


        [JsonProperty("rt"), JsonRequired()]
        [MinLength(1), StringLength(64)]
        public IList<string> ResourceTypes { get; set; } = new ObservableCollection<string>();

        public virtual bool ShouldSerializeInterfaces() { return true; }

        [JsonProperty("if", ItemConverterType = typeof(Newtonsoft.Json.Converters.StringEnumConverter))]

        public IList<OicResourceInterface> Interfaces { get; set; } = new ObservableCollection<OicResourceInterface>();
        public virtual bool ShouldSerializeName() { return true; }

        private string _name;

        [JsonProperty("n", NullValueHandling = NullValueHandling.Ignore)]
        public string Name
        {
            get => _name;
            set
            {
                if (_name == value)
                    return;
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public virtual bool ShouldSerializeId() { return true; }

        private string _id;

        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id
        {
            get => _id;
            set
            {
                if (_id == value)
                    return;
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }
        
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion


        public OicCoreResource()
        {
            
        }

        public override bool Equals(object obj)
        {
            var other = obj as OicCoreResource;

            if (other == null)
                return false;

            if (!string.Equals(RelativeUri, other.RelativeUri))
                return false;
            if (!ResourceTypes.NullRespectingSequenceEqual(other.ResourceTypes))
                return false;
            if (!Interfaces.NullRespectingSequenceEqual(other.Interfaces))
                return false;
            if (!string.Equals(Name, other.Name))
                return false;
            if (!string.Equals(Id, other.Id))
                return false;
            return true;
        }

        /// <summary>
        /// Gets a hashcode unique to the <see cref="OicCoreResource"/> sub-class. 
        /// </summary>
        /// <remarks>
        /// This will generate and store the hashcode based on the subclass's full name. 
        /// </remarks>
        private static readonly Dictionary<Type, int> _hashCode = new Dictionary<System.Type, int>();
        public override int GetHashCode()
        {
            if (_hashCode.TryGetValue(GetType(), out int hashcode) == false)
            {
                var fullName = GetType().FullName ?? throw new InvalidOperationException();
                hashcode = fullName.GetHashCode();
                _hashCode.Add(GetType(), hashcode);
            }

            return hashcode;
        }
    }

#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    public class OicBaseResouece<TValue> : OicCoreResource
    {
        [JsonProperty("value", Required = Required.Always, Order = 5)]
        public TValue Value { get; set; }

        public override void UpdateFields(IOicResource source)
        {
            var baseResource = source as OicBaseResouece<TValue> ?? throw ExceptionUtil.CreateResourceCastException<OicCoreResource>(nameof(source));
            base.UpdateFields(source);

            Value = baseResource.Value;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            var other = obj as OicBaseResouece<TValue>;
            if (other == null)
                return false;
            if (!base.Equals(obj))
                return false;
            if (!Value.Equals(other.Value))
                return false;
            return true;
        }
    }

    public class OicIntResouece : OicBaseResouece<int>
    {
        [JsonProperty("range", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore, Order = 6)]
        public List<int> Range { get; set; }

        public override void UpdateFields(IOicResource source)
        {
            var baseResource = source as OicIntResouece ?? throw ExceptionUtil.CreateResourceCastException<OicIntResouece>(nameof(source));
            base.UpdateFields(source);

            Range = baseResource.Range ?? Range;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            var other = obj as OicIntResouece;
            if (other == null)
                return false;
            if (!base.Equals(obj))
                return false;
            if (!Range.NullRespectingSequenceEqual(other.Range))
                return false;
            return true;
        }
    }

    public class OicNumberResouece : OicBaseResouece<float>
    {
        [JsonProperty("range", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore, Order = 6)]
        public List<float> Range { get; set; }

        public override void UpdateFields(IOicResource source)
        {
            var baseResource = source as OicNumberResouece ?? throw ExceptionUtil.CreateResourceCastException<OicNumberResouece>(nameof(source));
            base.UpdateFields(source);

            Range = baseResource.Range ?? Range;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            var other = obj as OicNumberResouece;
            if (other == null)
                return false;
            if (!base.Equals(obj))
                return false;
            if (!Range.NullRespectingSequenceEqual(other.Range))
                return false;
            return true;
        }
    }
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
}
