using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

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

    // TODO: review OIC Core Specification v1.1.1 section 12.3: Content Encoding in CBOR 
    public class OicMessageSerialiser
    {
        private readonly ResourceTypeResolver _resolver;

        public OicMessageSerialiser(ResourceTypeResolver resolver)
        {
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
        }

        public string Prettify(byte[] message, OicMessageContentType contentType)
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
                    throw new NotImplementedException($"Unsupported deserialisation of {contentType}");
            }
            return token.ToString(Formatting.Indented);
        }

        /// <summary>
        /// Deserialses a OIC message into a object based on the message's resource-type ("rt") property
        /// </summary>
        /// <param name="message"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public OicResourceList Deserialise(byte[] message, OicMessageContentType contentType)
            => new OicResourceList(DeserialiseInternal(message,contentType));

        private IEnumerable<IOicResource> DeserialiseInternal(byte[] message, OicMessageContentType contentType)
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
                    throw new NotImplementedException($"Unsupported deserialisation of {contentType}");
            }
            if (token.Type == JTokenType.Array)
                token = token.First;
            while (token != null)
            {
                if(token["rt"] == null)
                    throw new InvalidDataException("Key \"rt\" was not present in the message to deserialise");
                var rt = token["rt"].Select(t => (string)t);
                if(!_resolver.TryGetResourseType(rt, out var type))
                    throw new NotImplementedException($"Unknow resource types {token["rt"].ToString()} are not supported."); //Todo: fail gracefully instead of hard faulting while deserialising.
                yield return (IOicResource) token.ToObject(type);

                token = token.Next;
            }
        }

        //Todo: Enumerate all base calsses, extracting out their ResourceType to generate a "fall-back" array for the "rt" property
        public byte[] Serialise(IOicSerialisableResource resource, OicMessageContentType contentType)
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
                    throw new NotImplementedException($"Can not serialise unsupported content type ({contentType:G})");
            }
            return writer.ToArray();
        }
    }
}
