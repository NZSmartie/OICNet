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
                { "oic.r.core", typeof(OicCoreResource) }
            };
        }

        public Type GetResourseType(string id)
        {
            return _resourceTypes[id];
        }
    }
}
