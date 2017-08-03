using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Cbor;
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
        public Object Deserialise(byte[] message, OicMessageContentType contentType)
        {
            var serialiser = new JsonSerializer();
            var stream = new MemoryStream(message);
            OicCoreResource coreResource;
            Type type;
            switch (contentType)
            {
                case OicMessageContentType.ApplicationJson:
                {
                    coreResource =
                        serialiser.Deserialize<OicCoreResource>(new JsonTextReader(new StreamReader(stream)));
                    type = _resolver.GetResourseType(coreResource.ResourceTypes.FirstOrDefault());

                    stream.Seek(0, SeekOrigin.Begin);
                    return serialiser.Deserialize(new JsonTextReader(new StreamReader(stream)), type);
                }
                case OicMessageContentType.ApplicationCbor:
                {
                    coreResource = serialiser.Deserialize<OicCoreResource>(new CborDataReader(stream));
                    type = _resolver.GetResourseType(coreResource.ResourceTypes.FirstOrDefault());

                    stream.Seek(0, SeekOrigin.Begin);
                    return serialiser.Deserialize(new CborDataReader(stream), type);
                }
                default:
                    throw new NotImplementedException();
            }
        }

        public byte[] Serialise(OicCoreResource resource, OicMessageContentType contentType)
        {
            var writer = new MemoryStream();
            switch (contentType)
            {
                case OicMessageContentType.ApplicationJson:
                    StreamWriter sw = new StreamWriter(writer);
                    JsonSerializer.CreateDefault().Serialize(sw, resource);
                    sw.Flush();
                    break;
                case OicMessageContentType.ApplicationCbor:
                    new JsonSerializer().Serialize(new CborDataWriter(writer), resource);
                    break;
                default:
                    throw new NotImplementedException();
            }
            return writer.ToArray();
        }
    }
}
