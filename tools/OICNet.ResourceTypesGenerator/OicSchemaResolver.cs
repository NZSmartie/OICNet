using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace OICNet.ResourceTypesGenerator
{
    public class OicSchemaResolver : JSchemaResolver
    {
        public static readonly Uri[] DefaultBaseUris =
            {
                new Uri("https://www.openconnectivity.org/ocf-apis/core/schemas/"),
                new Uri("http://openinterconnect.org/iotdatamodels/schemas/")
            };

        private readonly List<Uri> _baseUris = new List<Uri>(DefaultBaseUris);

        private readonly Dictionary<string, string> _schemaCache = new Dictionary<string, string>();

        public IReadOnlyList<Uri> BaseUris => _baseUris;

        private Dictionary<string,JSchema> _definitions = new Dictionary<string, JSchema>();
        public IReadOnlyDictionary<string, JSchema> Definitions => _definitions;

        public void AddBaseUri(Uri uri)
        {
            _baseUris.Add(uri);
        }

        public void Add(Stream stream)
        {
            using (var reader = new JsonTextReader(new StreamReader(stream)) { CloseInput = false })
                AddInternal(JToken.Load(reader));
        }

        public void Add(string path)
        {
            using (var reader = new JsonTextReader(new StreamReader(path)))
                AddInternal(JToken.Load(reader));
        }

        public void AddInternal(JToken token)
        {
            var id = new UriBuilder(token["id"].Value<string>()) { Fragment = null }.Uri;

            // Assume we havea filename?
            var filename = Path.GetFileName(id.LocalPath);

            var baseUri = new UriBuilder(id)
            {
                Path = id.LocalPath.Substring(0, id.LocalPath.Length - filename.Length),
                Fragment = null
            }.Uri;

            if (!_baseUris.Any(b => Uri.Compare(b, baseUri, UriComponents.Scheme | UriComponents.HostAndPort | UriComponents.Path, UriFormat.UriEscaped, StringComparison.OrdinalIgnoreCase) == 0))
            {
                Debug.WriteLine($"Adding new base URI ({baseUri})");
                _baseUris.Add(baseUri);
            }

            _schemaCache.Add(filename, token.ToString(Formatting.None));
        }

        public override Stream GetSchemaResource(ResolveSchemaContext context, SchemaReference reference)
        {
            var contextFilename = Path.GetFileName(context.ResolvedSchemaId.LocalPath);
            var reducedUri = new UriBuilder(context.ResolvedSchemaId)
            {
                Fragment = null,
                Path = context.ResolvedSchemaId.LocalPath.Substring(0, context.ResolvedSchemaId.LocalPath.Length - contextFilename.Length)
            }.Uri;

            if (!_baseUris.Any(b => Uri.Compare(b, reducedUri, UriComponents.Scheme | UriComponents.HostAndPort | UriComponents.Path, UriFormat.UriEscaped, StringComparison.OrdinalIgnoreCase) == 0))
            {
                Debug.WriteLine($"Failed to find matching baseuri for {reducedUri}");
                return null;
            }
    
            if(!_schemaCache.TryGetValue(contextFilename, out var schemaData))
            {
                Debug.WriteLine($"Failed to find schema for {contextFilename}");
                return null;
            }

            return new MemoryStream(Encoding.UTF8.GetBytes(schemaData), false);
        }
    }
}