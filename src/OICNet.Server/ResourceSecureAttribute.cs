using System;
using System.Collections.Generic;
using System.Text;

namespace OICNet.Server
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ResourceSecureAttribute : Attribute
    {
        public ResourceSecureAttribute()
        {

        }
    }
}
