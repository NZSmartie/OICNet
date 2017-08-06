using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Cbor;
using Newtonsoft.Json.Cbor.Linq;
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
        /// Todo: Support multiple resource types some how... Currently only supports the first resource type, any subsequent types are ignored.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public IEnumerable<IOicResource> Deserialise(byte[] message, OicMessageContentType contentType)
        {
            var stream = new MemoryStream(message);
            JToken token;

            switch (contentType)
            {
                case OicMessageContentType.ApplicationJson:
                    token = JToken.ReadFrom(new JsonTextReader(new StreamReader(stream)));
                    break;
                case OicMessageContentType.ApplicationCbor:
                    token = JToken.ReadFrom(new CborDataReader(stream));
                    break;
                default:
                    throw new NotImplementedException();
            }
            if (token.Type == JTokenType.Array)
                token = token.First;
            while (token != null)
            {
                var rt = (string) token["rt"].FirstOrDefault();
                var type = _resolver.GetResourseType(rt);
                yield return (IOicResource) token.ToObject(type);

                token = token.Next;
            }
        }

        //Todo: support serialising multiple IOicResouces in an array/list
        public byte[] Serialise(IOicResource resource, OicMessageContentType contentType)
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
