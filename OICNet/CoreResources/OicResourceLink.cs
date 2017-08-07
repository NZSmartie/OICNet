using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OICNet;

namespace OICNet.CoreResources
{
    [Flags]
    public enum LinkPolicyFlags : byte
    {
        /// <summary>
        /// The discoverable rule defines whether the <see cref="OicResourceLink"/> is to be included in the Resource discovery message via /oic/res.
        /// </summary>
        Discoverable = 0x01,
        /// <summary>
        /// The observable rule defines whether the Resource referenced by the target URI supports the NOTIFY operation.
        /// </summary>
        Observable = 0x02
    }

#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    public class OicResourceLink
    {
        public class LinkPolicies
        {
            /// <summary>
            /// Specifies the framework policies on the Resource referenced by the target URI
            /// </summary>
            // Todo: Create a flag enum
            [JsonProperty("bm", Required = Required.Always)]
            public LinkPolicyFlags Policies { get; set; }

            /// <summary>
            /// Specifies if security needs to be turned on when looking to interact with the Resource
            /// </summary>
            [JsonProperty("sec")]
            public bool IsSecure { get; set; }

            /// <summary>
            /// Secure port to be used for connection
            /// </summary>
            [JsonProperty("port")]
            public int SecurePort { get; set; }

            public override bool Equals(object obj)
            {
                var other = obj as LinkPolicies;
                return other != null && Policies == other.Policies && IsSecure == other.IsSecure &&
                       SecurePort == other.SecurePort;
            }
        }

        /// <summary>
        /// This is the target URI, it can be specified as a Relative Reference or fully-qualified URI. Relative Reference should be used along with the di parameter to make it unique.
        /// </summary>
        [JsonProperty("href", Required = Required.Always), StringLength(256)]
        public Uri Href { get; set; }

        /// <summary>
        /// The relation of the target URI referenced by the link to the context URI
        /// </summary>
        [JsonProperty("rel", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore), StringLength(64)]
        public string Rel { get; set; } = "hosts";

        [JsonProperty("rt"), JsonRequired()]
        [MinLength(1), StringLength(64)]
        public List<string> ResourceTypes { get; set; }

        /// <summary>
        /// The interface set supported by this resource
        /// </summary>
        [JsonProperty("if", ItemConverterType = typeof(Newtonsoft.Json.Converters.StringEnumConverter)), JsonRequired()]
        public List<OicResourceInterface> Interfaces { get; set; }

        /// <summary>
        /// The Device ID on which the Relative Reference in href is to be resolved on. Base URI should be used in preference where possible
        /// </summary>
        [JsonProperty("di", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Guid DeviceId { get; set; }

        /// <summary>
        /// The base URI used to fully qualify a Relative Reference in the href parameter. Use the OCF Schema for URI
        /// </summary>
        [JsonProperty("buri", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore), StringLength(256)]
        public Uri BaseUri { get; set; }

        /// <summary>
        /// Specifies the framework policies on the Resource referenced by the target URI
        /// </summary>
        [JsonProperty("p", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public LinkPolicies Policies { get; set; }
        
        /// <summary>
        /// URI parameters to use with an <see cref="OicResourceInterface.Batch"/> batch request using this <see cref="OicResourceLink"/>
        /// </summary>
        [JsonProperty("bp", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string BatchParameters { get; set; }

        /// <summary>
        /// A title for the link relation. Can be used by the UI to provide a context
        /// </summary>
        [JsonProperty("title", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore), StringLength(64)]
        public string Title { get; set; }

        /// <summary>
        /// This is used to override the context URI e.g. override the URI of the containing collection
        /// </summary>
        [JsonProperty("anchor", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore), StringLength(256)]
        public Uri Anchor { get; set; }

        /// <summary>
        /// The instance identifier for this web link in an array of web links - used in collections
        /// <para><see cref="Guid"/> - Use UUID for universal uniqueness - used in /oic/res to identify the device</para>
        /// <para><see cref="Uri"/> - Any unique string including a URI</para>
        /// <para><see cref="int"/> - An ordinal number that is not repeated - must be unique in the collection context</para>
        /// </summary>
        // Todo: verify this type is either a GUID, Uri(maxLen=256) or int
        [JsonProperty("ins", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public JValue InstanceId { get; set; }

        /// <summary>
        /// A hint at the representation of the resource referenced by the target URI. This represents the media types that are used for both accepting and emitting
        /// </summary>
        [JsonProperty("type", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public List<string> TypeHints { get; set; } = new List<string>{ "application/cbor" };

        public override bool Equals(object obj)
        {
            var other = obj as OicResourceLink;
            if (other == null)
                return false;
            //if (!base.Equals(obj))
            //    return false;
            if (Href != other.Href)
                return false;
            if (Rel != other.Rel)
                return false;
            if (!ResourceTypes.SequenceEqual(other.ResourceTypes))
                return false;
            if (!Interfaces.SequenceEqual(other.Interfaces))
                return false;
            if (DeviceId != other.DeviceId)
                return false;
            if (BaseUri != other.BaseUri)
                return false;
            if (Policies != other.Policies)
                return false;
            if (BatchParameters != other.BatchParameters)
                return false;
            if (Title != other.Title)
                return false;
            if (Anchor != other.Anchor)
                return false;
            if (InstanceId?.Value != other.InstanceId?.Value)
                return false;
            if (!TypeHints.SequenceEqual(other.TypeHints))
                return false;

            return true;
        }

        public IOicResource CreateResource(OicResolver resolver)
        {
            if (!resolver.TryGetResourseType(ResourceTypes, out var type))
                throw new NotImplementedException($"Unsupported resource types [\"{string.Join("\", ", ResourceTypes)}\"]");
            var resource = (IOicResource)Activator.CreateInstance(type);

            resource.Name = Title;
            resource.RelativeUri = Href.OriginalString; // Todo: Figure out how to get the relative path from a Resource Link and not assume OriginalString will always work
            resource.ResourceTypes = new List<string>(ResourceTypes);
            resource.Interfaces = new List<OicResourceInterface>(Interfaces);

            return resource;
        }
    }
}
