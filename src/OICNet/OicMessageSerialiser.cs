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
            using (var stream = new MemoryStream(message))
                return GetJToken(stream, contentType).ToString(Formatting.Indented);
        }

        public string Prettify(Stream stream, OicMessageContentType contentType)
        {
            return GetJToken(stream, contentType).ToString(Formatting.Indented);
        }

        private JToken GetJToken(Stream stream, OicMessageContentType contentType)
        {
            switch (contentType)
            {
                case OicMessageContentType.ApplicationJson:
                    return JToken.ReadFrom(new JsonTextReader(new StreamReader(stream)));
                case OicMessageContentType.ApplicationCbor:
                    return JToken.ReadFrom(new CborDataReader(stream));
                default:
                    throw new NotImplementedException($"Unsupported deserialisation of {contentType}");
            }
        }

        /// <summary>
        /// Deserialses a OIC message into a object based on the message's resource-type ("rt") property
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public IOicSerialisableResource Deserialise(Stream stream, OicMessageContentType contentType)
        {
            var token = GetJToken(stream, contentType);

            if (token.Type == JTokenType.Array)
                return new OicResourceList(DeserialiseListInternal(token.First));

            return DeserialiseInternal(token);
        }

        /// <summary>
        /// Deserialses a OIC message into a object based on the message's resource-type ("rt") property
        /// </summary>
        /// <param name="message"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public IOicSerialisableResource Deserialise(byte[] message, OicMessageContentType contentType)
        {
            using (var stream = new MemoryStream(message))
                return Deserialise(stream, contentType);
        }

        private IOicResource DeserialiseInternal(JToken token)
        {
            if (token["rt"] == null)
                throw new InvalidDataException("Key \"rt\" was not present in the message to deserialise");
            var rt = token["rt"].Select(t => (string)t);
            if (!_resolver.TryGetResourseType(rt, out var type))
                throw new NotImplementedException($"Unknow resource types {token["rt"].ToString()} are not supported."); //Todo: fail gracefully instead of hard faulting while deserialising.
            return (IOicResource)token.ToObject(type);
        }

        private IEnumerable<IOicResource> DeserialiseListInternal(JToken token)
        {
            while (token != null)
            {
                yield return DeserialiseInternal(token);
                token = token.Next;
            }
        }

        //Todo: Enumerate all base calsses, extracting out their ResourceType to generate a "fall-back" array for the "rt" property
        public byte[] Serialise(IOicSerialisableResource resource, OicMessageContentType contentType)
        {
            using (var writer = new MemoryStream())
            {
                Serialise(writer, resource, contentType);
                return writer.ToArray();
            }
        }

        //Todo: Enumerate all base calsses, extracting out their ResourceType to generate a "fall-back" array for the "rt" property
        public void Serialise(Stream stream, IOicSerialisableResource resource, OicMessageContentType contentType)
        {
            switch (contentType)
            {
                case OicMessageContentType.ApplicationJson:
                    StreamWriter sw = new StreamWriter(stream);
                    JsonSerializer.CreateDefault().Serialize(sw, resource);
                    sw.Flush();
                    break;
                case OicMessageContentType.ApplicationCbor:
                    new JsonSerializer().Serialize(new CborDataWriter(stream), resource);
                    break;
                default:
                    throw new NotImplementedException($"Can not serialise unsupported content type ({contentType:G})");
            }
            stream.Flush();
        }
    }
}
