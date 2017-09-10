using System.Collections.Generic;
using System.Reflection;

namespace OICNet.Server.Mvc.ApplicationParts
{
    public interface IApplicationPartTypeProvider
    {
        IEnumerable<TypeInfo> Types { get; }
    }
}