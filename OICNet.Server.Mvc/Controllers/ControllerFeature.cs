using System.Collections.Generic;
using System.Reflection;

namespace OICNet.Server.Mvc.Controllers
{
    public class ControllerFeature
    {
        public IList<TypeInfo> Controllers { get; } = new List<TypeInfo>();
    }
}