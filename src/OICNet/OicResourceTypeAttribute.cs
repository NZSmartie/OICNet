using System;

namespace OICNet
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class OicResourceTypeAttribute : Attribute
    {
        public readonly string Id;
        
        public OicResourceTypeAttribute(string id)
        {
            Id = id;
        }
    }
}
