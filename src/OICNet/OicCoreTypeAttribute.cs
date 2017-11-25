using System;

namespace OICNet
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class OicCoreTypeAttribute : Attribute
    {
        public readonly string Id;
        
        public OicCoreTypeAttribute(string id)
        {
            Id = id;
        }
    }
}
