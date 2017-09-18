using System;
using System.Collections.Generic;
using System.Text;

namespace OICNet.Server
{
    [Flags]
    public enum OicResourcePolicies
    {
        None = 0x00,
        Discoverable = 0x01,
        Observable = 0x02,
        Secure = 0x04
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class OicResourceAttribute : Attribute
    {
        public OicResourcePolicies Policies { get; }

        public OicResourceAttribute(OicResourcePolicies policies)
        {
            Policies = policies;
        }

    }
}
