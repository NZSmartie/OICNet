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
using System.Reflection;
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
        Assembly Generate();
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

            private readonly Dictionary<string, Type> _subTypes = new Dictionary<string, Type>();
            private readonly HashSet<string> _propertiesToSkip = new HashSet<string>();
            private CodeCompileUnit _codeUnit;

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
                GenerateInternal();

                using (var tw = new IndentedTextWriter(output, "    "))
                {
                    new CSharpCodeProvider().GenerateCodeFromCompileUnit(_codeUnit, tw, new CodeGeneratorOptions());
                }
                output.Flush();
            }

            public Assembly Generate()
            {
                GenerateInternal();

                var compilerParameters = new CompilerParameters(Assembly.GetExecutingAssembly().GetReferencedAssemblies().Select(a => a.Name).ToArray())
                {
                    GenerateInMemory = true,
                    GenerateExecutable = false
                };

                var result = new CSharpCodeProvider().CompileAssemblyFromDom(compilerParameters, _codeUnit);

                return result.CompiledAssembly;
            }

            protected void GenerateInternal()
            {
                this._codeUnit = new CodeCompileUnit();

                _codeUnit.Namespaces.Add(new CodeNamespace()
                {
                    Imports = {
                        new CodeNamespaceImport("Newtonsoft.Json"),
                        new CodeNamespaceImport("OICNet"),
                        new CodeNamespaceImport("System.Collections.Generic"),
                        new CodeNamespaceImport("System.ComponentModel.DataAnnotations"),
                    }
                });

                var codeNamespace = new CodeNamespace(Namespace);
                _codeUnit.Namespaces.Add(codeNamespace);

                // TODO: Get /All/ resource types (rt)
                var id = Path.GetFileNameWithoutExtension(_schema.Id.LocalPath);

                var codeClass = new CodeTypeDeclaration(id.ToCapitalCase());
                codeNamespace.Types.Add(codeClass);

                codeClass.Comments.Add(new CodeCommentStatement(
                    $"<summary>\n " +
                    $"Auto-generated class to match the supplied schema ({_schema.Id})\n " + 
                    $"<para>\n {_schema.Description}\n <para>\n " + 
                     "</summary>", true)
                );

                // Add custom attribute (e.g. [OicResourceType("oic.r.audio")] )
                codeClass.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(OicResourceTypeAttribute)),
                    new CodeAttributeArgument(new CodePrimitiveExpression(id))));


                foreach (var schema in _schema.AllOf)
                {
                    var sfpjggdfjgn = schema.Id;

                    //// Add in requests sub-types
                    //foreach (var st in _subTypes)
                    //    codeClass.BaseTypes.Add(st);

                    foreach (var schemaProperty in schema.Properties)
                    {
                        if (_propertiesToSkip.Contains(schemaProperty.Key))
                        {
                            Debug.WriteLine($"{schemaProperty.Key} is already decalred in an inherited type");
                            continue;
                        }

                        var property = new CodeMemberProperty();
                        var field = new CodeMemberField();
                        if (PropertyAliases.TryGetValue(schemaProperty.Key, out var n))
                            field.Name = (property.Name = n).ToPrivateName();
                        else
                            field.Name = (property.Name = schemaProperty.Key.ToCapitalCase()).ToPrivateName();

                        property.Attributes = MemberAttributes.Public | MemberAttributes.VTableMask;
                        field.Attributes = MemberAttributes.Private;

                        var jsonProperty = new CodeAttributeDeclaration(new CodeTypeReference(typeof(JsonPropertyAttribute)));
                        jsonProperty.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(schemaProperty.Key)));

                        if (_schema.Required.Contains(schemaProperty.Key) || schema.Required.Contains(schemaProperty.Key))
                        {
                            jsonProperty.Arguments.Add(new CodeAttributeArgument(
                                
                                nameof(JsonPropertyAttribute.Required),
                                new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(new CodeTypeReference(typeof(Required))), nameof(Required.Always))
                            ));
                        }

                        property.CustomAttributes.Add(jsonProperty);

                        if (schemaProperty.Value.MinimumItems.HasValue)
                            property.CustomAttributes.Add(new CodeAttributeDeclaration(
                                new CodeTypeReference(typeof(MinLengthAttribute)),
                                new CodeAttributeArgument(new CodePrimitiveExpression(schemaProperty.Value.MinimumItems.Value))));

                        if (schemaProperty.Value.MaximumItems.HasValue)
                            property.CustomAttributes.Add(new CodeAttributeDeclaration(
                                 new CodeTypeReference(typeof(MaxLengthAttribute)),
                                new CodeAttributeArgument(new CodePrimitiveExpression(schemaProperty.Value.MaximumItems.Value))));

                        if (schemaProperty.Value.MaximumLength.HasValue)
                            property.CustomAttributes.Add(new CodeAttributeDeclaration(
                                new CodeTypeReference(typeof(StringLengthAttribute)),
                                new CodeAttributeArgument(new CodePrimitiveExpression(schemaProperty.Value.MaximumLength.Value))));

                        if (schemaProperty.Value.Minimum.HasValue || schemaProperty.Value.Maximum.HasValue)
                        {
                            var minimum = schemaProperty.Value.Minimum.HasValue
                                    ? schemaProperty.Value.Minimum.Value : double.NegativeInfinity;

                            var maximum = schemaProperty.Value.Maximum.HasValue
                                    ? schemaProperty.Value.Maximum.Value : double.PositiveInfinity;

                            var rangeRattribute = new CodeAttributeDeclaration(
                                new CodeTypeReference(typeof(RangeAttribute)),
                                new CodeAttributeArgument(new CodePrimitiveExpression(minimum)),
                                new CodeAttributeArgument(new CodePrimitiveExpression(maximum)));

                            property.CustomAttributes.Add(rangeRattribute);
                        }



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
                        field.Type = property.Type = GetTypeForProperty(schemaProperty.Value, property);

                        //property.Comments.Add(new CodeCommentStatement(
                        //    $"<remarks>\n Source: {schemaProperty}\n </remarks>", true));

                        property.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), field.Name)));
                        property.SetStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), field.Name), new CodePropertySetValueReferenceExpression()));

                        codeClass.Members.Add(field);
                        codeClass.Members.Add(property);
                    }
                }

                // TODO: associate JSON attributes 

                // TODO: Somehow expose metadata about properties like readonly, range, valid values

                // TODO: Add XMLDoc based on descriptions provided

                CodeGenerator.ValidateIdentifiers(_codeUnit);
            }

            protected CodeTypeReference GetTypeForProperty(JSchema propertySchema, CodeMemberProperty property)
            {
                var propertyType = propertySchema.Type.GetValueOrDefault(JSchemaType.None);
                switch (propertyType)
                {
                    case JSchemaType.Boolean:
                        return new CodeTypeReference(typeof(bool));
                    case JSchemaType.Integer:
                        return new CodeTypeReference(typeof(int));
                    case JSchemaType.Number:
                        return new CodeTypeReference(typeof(decimal));
                    case JSchemaType.String:
                        return new CodeTypeReference(typeof(string));
                    case JSchemaType.Array:
                        if (propertySchema.Items.Count != 1)
                            throw new NotImplementedException("Not sure what to do with arrrrrrray of items for a schema property");

                        return new CodeTypeReference(typeof(IList))
                        {
                            TypeArguments = { GetTypeForProperty(propertySchema.Items.First(), property) }
                        };
                    case JSchemaType.None:
                        property.Comments.Add(new CodeCommentStatement(
                            $"<remarks>\n WARNING: Possibly unsupported types. Specifically 'anyOf'.\n </remarks>", true));

                        // Revert to JValue as it leaves it up to the JSON library to fill in the blank for us and allows the consuming application to still get what they want
                        // TODO: Ensure values are one of the "anyOf" types defined in the schema
                        return  new CodeTypeReference(typeof(JValue));
                    default:
                        throw new NotImplementedException($"Code's borken y'all: {propertyType}");
                }
            }

            public IResourceTypeGenerator SubType(Type baseType)
            {
                var coreTypeAttribute = baseType.GetCustomAttribute<OicCoreTypeAttribute>()
                    ?? throw new ArgumentException($"{baseType} is not decorated with a custom [OicCoreType] attribute");

                _subTypes.Add(coreTypeAttribute.Id, baseType);
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
