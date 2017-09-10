using System;

namespace OICNet.Server
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class OicControllerAttribute : Attribute
    {
    }
}