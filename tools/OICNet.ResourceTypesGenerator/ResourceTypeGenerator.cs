using Microsoft.CSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace OICNet.ResourceTypesGenerator
{
    public static class ResourceTypeGeneratorExtensions
    {
        public static IResourceTypeGenerator With(this ResourceTypeGenerator generator, string schemaFilePath)
        {
            using (var reader = File.OpenText(schemaFilePath))
            {
                return generator.With(reader);
            }
        }

        public static IResourceTypeGenerator SubType<TBase>(this IResourceTypeGenerator generator)
        {
            return generator.SubType(typeof(TBase));
        }

        public static void Generate(this IResourceTypeGenerator generator, string sourceCodePath)
        {
            var file = new FileInfo(sourceCodePath);

            if (!file.Directory.Exists)
                file.Directory.Create();

            using (var writer = new StreamWriter(file.Open(FileMode.Create)))
            {
                generator.Generate(writer);
                writer.Flush();
            }
        }
    }

    public interface IResourceTypeGenerator
    {
        IResourceTypeGenerator SubType(Type baseType);
        void Generate(StreamWriter output);
        IResourceTypeGenerator UseNamespace(string @namespace);
        IResourceTypeGenerator UseAlias(string key, string alias);
    }

    public class ResourceTypeGenerator
    {
        private readonly OicSchemaResolver _jSchemaResolver;
        private readonly JSchemaReaderSettings _jSchemaSettings;

        public string Namespace { get; set; }

        public Dictionary<string, string> PropertyAliases { get; } = new Dictionary<string, string>();

        //private readonly JSchemaReaderSettings _settings = OicSchemaResolver.DefaultSettings;

        public ResourceTypeGenerator(OicSchemaResolver jSchemaResolver)
        {
            _jSchemaResolver = jSchemaResolver;

            _jSchemaSettings = new JSchemaReaderSettings
            {
                Resolver = _jSchemaResolver,
            };
        }

        public ResourceTypeGenerator UseNamespace(string @namespace)
        {
            Namespace = @namespace;
            return this;
        }

        public ResourceTypeGenerator UseAlias(string key, string alias)
        {
            PropertyAliases.Add(key, alias);
            return this;
        }

        public IResourceTypeGenerator With(StreamReader stream)
        {
            using (var reader = new JsonTextReader(stream))
            {
                var schema = JSchema.Load(reader, _jSchemaSettings);

                return new ResourceTypeGeneratorInternal(this, schema)
                {
                    Namespace = Namespace,
                    PropertyAliases = PropertyAliases
                    // Setup geneator by specifying target schema
                };
            }
        }

        internal class ResourceTypeGeneratorInternal : IResourceTypeGenerator
        {
            private readonly ResourceTypeGenerator _parent;
            private readonly JSchema _schema;

            private readonly List<Type> _subTypes = new List<Type>();
            private readonly HashSet<string> _propertiesToSkip = new HashSet<string>();

            public ResourceTypeGeneratorInternal(ResourceTypeGenerator parent, JSchema schema)
            {
                _parent = parent;
                _schema = schema;
            }

            public string Namespace { get; set; }
            public Dictionary<string, string> PropertyAliases { get; set; }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="output"></param>
            public void Generate(StreamWriter output)
            {
                var codeUnit = new CodeCompileUnit();

                codeUnit.Namespaces.Add(new CodeNamespace()
                {
                    Imports = {
                        new CodeNamespaceImport("Newtonsoft.Json"),
                        new CodeNamespaceImport("OICNet"),
                        new CodeNamespaceImport("System.Collections.Generic"),
                        new CodeNamespaceImport("System.ComponentModel.DataAnnotations"),
                    }
                });

                var codeNamespace = new CodeNamespace(Namespace);
                codeUnit.Namespaces.Add(codeNamespace);

                // TODO: Get /All/ resource types (rt)
                var id = Path.GetFileNameWithoutExtension(_schema.Id.LocalPath);

                var codeClass = new CodeTypeDeclaration(id.ToCapitalCase());
                codeNamespace.Types.Add(codeClass);

                codeClass.Comments.Add(new CodeCommentStatement(
                    $"<summary>\n " +
                    $"Auto-generated class to match the supplied schema ({_schema.Id})\n\n " +
                    $"<para>{_schema.Description}<para>\n " +
                    $"</summary>", true)
                );

                // Add in requests sub-types
                foreach(var st in _subTypes)
                    codeClass.BaseTypes.Add(st);

                // Add custom attribute (e.g. [OicResourceType("oic.r.audio")] )
                codeClass.CustomAttributes.Add(new CodeAttributeDeclaration(nameof(OicResourceTypeAttribute).WithoutAttributeSuffix(),
                    new CodeAttributeArgument(new CodePrimitiveExpression(id))));


                foreach (var schema in _schema.AllOf)
                {
                    foreach(var schemaProperty in schema.Properties)
                    {
                        if (_propertiesToSkip.Contains(schemaProperty.Key))
                        {
                            Debug.WriteLine($"{schemaProperty.Key} is already decalred in an inherited type");
                            continue;
                        }

                        var property = new CodeMemberProperty();
                        if (PropertyAliases.TryGetValue(schemaProperty.Key, out var n))
                            property.Name = n;
                        else
                            property.Name = schemaProperty.Key.ToCapitalCase();

                        property.Attributes = MemberAttributes.Public | MemberAttributes.VTableMask;

                        var jsonProperty = new CodeAttributeDeclaration(nameof(JsonPropertyAttribute).WithoutAttributeSuffix());
                        jsonProperty.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(schemaProperty.Key)));

                        if (_schema.Required.Contains(schemaProperty.Key) || schema.Required.Contains(schemaProperty.Key))
                        {
                            jsonProperty.Arguments.Add(new CodeAttributeArgument(
                                nameof(JsonPropertyAttribute.Required),
                                new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(nameof(Required)), nameof(Required.Always))
                            ));
                        }

                        property.CustomAttributes.Add(jsonProperty);

                        if (schemaProperty.Value.MinimumItems.HasValue)
                            property.CustomAttributes.Add(new CodeAttributeDeclaration(
                                nameof(MinLengthAttribute).WithoutAttributeSuffix(),
                                new CodeAttributeArgument(new CodePrimitiveExpression(schemaProperty.Value.MinimumItems.Value))));

                        if (schemaProperty.Value.MaximumItems.HasValue)
                            property.CustomAttributes.Add(new CodeAttributeDeclaration(
                                nameof(MaxLengthAttribute).WithoutAttributeSuffix(),
                                new CodeAttributeArgument(new CodePrimitiveExpression(schemaProperty.Value.MaximumItems.Value))));



                        if (schemaProperty.Value.ExtensionData.TryGetValue("readOnly", out var readonlyToken))
                        {
                            if (readonlyToken.Type != JTokenType.Boolean)
                                throw new JSchemaValidationException($"readOnly is not of type Boolean for the schema property {schemaProperty.Key}");

                            if ((bool)readonlyToken)
                                property.CustomAttributes.Add(new CodeAttributeDeclaration(nameof(OicJsonReadOnlyAttribute).WithoutAttributeSuffix()));
                        }

                        property.Comments.Add(new CodeCommentStatement(
                            $"<summary>\n {schemaProperty.Value.Description}\n </summary>", true));

                        // TODO: Get the correct type for the property. should be a subset of what's available in oic.core.json?
                        property.Type = GetTypeForProperty(schemaProperty.Value, property);

                        property.Comments.Add(new CodeCommentStatement(
                            $"<remarks>\n Source: {schemaProperty}\n </remarks>", true));

                        codeClass.Members.Add(property);
                    }
                }

                // TODO: associate JSON attributes 

                // TODO: Somehow expose metadata about properties like readonly, range, valid values

                // TODO: Add XMLDoc based on descriptions provided

                using (var tw = new IndentedTextWriter(output, "    "))
                {
                    new CSharpCodeProvider().GenerateCodeFromCompileUnit(codeUnit, tw, new CodeGeneratorOptions());
                }
                output.Flush();
            }

            protected CodeTypeReference GetTypeForProperty(JSchema propertySchema, CodeMemberProperty property)
            {
                var propertyType = propertySchema.Type.GetValueOrDefault(JSchemaType.None);
                switch (propertyType)
                {
                    case JSchemaType.Boolean:
                        return new CodeTypeReference(typeof(bool).FullName);
                    case JSchemaType.Integer:
                        return new CodeTypeReference(typeof(int).FullName);
                    case JSchemaType.Number:
                        return new CodeTypeReference(typeof(decimal).FullName);
                    case JSchemaType.String:
                        return new CodeTypeReference(typeof(string).FullName);
                    case JSchemaType.Array:
                        if (propertySchema.Items.Count != 1)
                            throw new NotImplementedException("Not sure what to do with arrrrrrray of items for a schema property");

                        return new CodeTypeReference(nameof(IList), GetTypeForProperty(propertySchema.Items.First(), property));
                    case JSchemaType.None:
                        property.Comments.Add(new CodeCommentStatement(
                            $"<remarks>\n WARNING: Possibly unsupported types. Specifically 'anyOf'.\n </remarks>", true));

                        // Revert to JValue as it leaves it up to the JSON library to fill in the blank for us and allows the consuming application to still get what they want
                        // TODO: Ensure values are one of the "anyOf" types defined in the schema
                        return  new CodeTypeReference(nameof(JValue));
                    default:
                        throw new NotImplementedException($"Code's borken y'all: {propertyType}");
                }
            }

            public IResourceTypeGenerator SubType(Type baseType)
            {
                if (!baseType.IsInterface && _subTypes.Any(st => !st.IsInterface))
                    throw new InvalidOperationException("Can not supply more than one abstract class or concrete base type");

                // TODO: Ummm... something somethings validate subtype usefulness or correctness?
                _propertiesToSkip.UnionWith(baseType.GetProperties()
                                                    .SelectMany(p => ((JsonPropertyAttribute[])p.GetCustomAttributes(typeof(JsonPropertyAttribute), false)))
                                                    .Select(k => k.PropertyName));

                _subTypes.Add(baseType);
                return this;
            }

            public IResourceTypeGenerator UseAlias(string key, string alias)
            {
                PropertyAliases.Add(key, alias);
                return this;
            }

            public IResourceTypeGenerator UseNamespace(string @namespace)
            {
                Namespace = @namespace;
                return this;
            }
        }
    }
}
