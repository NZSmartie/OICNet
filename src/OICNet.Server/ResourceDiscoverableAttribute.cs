using System;
using System.Collections.Generic;
using System.Text;

namespace OICNet.Server
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ResourceDiscoverableAttribute : Attribute
    {
        public ResourceDiscoverableAttribute()
        {

        }
    }
}
