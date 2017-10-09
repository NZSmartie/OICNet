

using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using NUnit.Framework;
using System;
using System.IO;

namespace OICNet.ResourceTypesGenerator.Tests
{
    [TestFixture]
    public class OicSchemaResolverTest
    {
        private OicSchemaResolver _jSchemaResolver;
        private JSchemaReaderSettings _jSchemaSettings;

        public const string OicCoreSchemaPath = @"Schemas\oic.core-schema.json";
        public const string OicBaseResourceSchemaPath = @"Schemas\oic.baseResource.json";

        [SetUp]
        public void Setup()
        {
            _jSchemaResolver = new OicSchemaResolver();
            _jSchemaSettings = new JSchemaReaderSettings
            {
                Resolver = _jSchemaResolver
            };
        }

        [Test]
        public void Preload_OicCoreSchema()
        {
            // Arrange
            _jSchemaResolver.Add(OicCoreSchemaPath);

            // Act - Test oic core schema by loading base-resource schema
            TestDelegate action = () => JSchema.Load(new JsonTextReader(new StreamReader(OicBaseResourceSchemaPath)), _jSchemaSettings);

            // Assert
            Assert.DoesNotThrow(action);
        }
    }
}
