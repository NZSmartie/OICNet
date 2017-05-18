using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OICNet
{
    public enum MediaType
    {
        TextPlain,
        ApplicationJson,
        ApplicationCbor,
        ApplicationXml
    }

    public class OicMessageSerialiser
    {
        IResourceTypeResolver _resolver;

        public OicMessageSerialiser(IResourceTypeResolver resolver)
        {
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
        }

        /// <summary>
        /// Deserialses a OIC message into a object based on the message's resource-type ("rt") property
        /// 
        /// Todo: Support multiple resource types some how... Currently only supports the first resource type, any subsequent types are ignored.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public Object Deserialise(string message)
        {
            var coreResource = JsonConvert.DeserializeObject<OicCoreResource>(message);
            var type = _resolver.GetResourseType(coreResource.ResourceTypes.FirstOrDefault());

            return JsonConvert.DeserializeObject(message, type);
        }

        public string Serialise(OicCoreResource resource)
        {
            return JsonConvert.SerializeObject(resource);
        }
    }
}
