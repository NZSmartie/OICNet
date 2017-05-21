using System;
using System.Collections.Generic;
using System.Text;

namespace OICNet
{
    public interface IResourceTypeResolver
    {
        Type GetResourseType(string id);
    }

    /// <summary>
    /// Default resource-type resolver for all of built in resource types
    /// </summary>
    /// <remarks>(Will) Support all OIC v1.1.0 defined resource-types</remarks>
    public class OicResolver : IResourceTypeResolver
    {
        private Dictionary<string, Type> _resourceTypes;

        public OicResolver()
        {
            // List of built in resource-types will go here (OIC v1.1.0)
            _resourceTypes = new Dictionary<string, Type>
            {
                // Todo: In .Net Standard 2.0, replace hardcoded references with reflection, looking for classes with OicResourceTypeAttribute
                { "oic.r.core", typeof(OicCoreResource) },
                { "oic.r.audio", typeof(ResourceTypes.Audio) },
                { "oic.r.automaticdocumentfeeder", typeof(ResourceTypes.AutomaticDocumentFeeder) },
                { "oic.r.door", typeof(ResourceTypes.Door) },
                { "oic.r.lock.status", typeof(ResourceTypes.LockStatus) },
                { "oic.r.media", typeof(ResourceTypes.Media) },
                { "oic.r.mediasource", typeof(ResourceTypes.MediaSource) },
                { "oic.r.mediasourcelist", typeof(ResourceTypes.MediaSourceList) },
                { "oic.r.media.input", typeof(ResourceTypes.MediaSourceList) },
                { "oic.r.media.output", typeof(ResourceTypes.MediaSourceList) },
                { "oic.r.mode", typeof(ResourceTypes.Mode) },
                { "oic.r.openlevel", typeof(ResourceTypes.OpenLevel) },
                { "oic.r.operational.state", typeof(ResourceTypes.OperationalState) },
                { "oic.r.switch.binary", typeof(ResourceTypes.SwitchBinary) },
                { "oic.r.temperature", typeof(ResourceTypes.Temperature) },

            };
        }

        public Type GetResourseType(string id)
        {
            return _resourceTypes[id];
        }
    }
}
