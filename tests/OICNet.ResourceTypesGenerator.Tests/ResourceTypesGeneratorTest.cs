using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using NUnit.Framework;
using System;
using System.IO;

namespace OICNet.ResourceTypesGenerator.Tests
{
    [TestFixture]
    public class ResourceTypesGeneratorTest
    {
        private OicSchemaResolver _jSchemaResolver;

        public const string OicCoreSchemaPath = @"Schemas\oic.core-schema.json";
        public const string OicCoreSchema2Path = @"Schemas\oic.core.json";
        public const string OicBaseResourceSchemaPath = @"Schemas\oic.baseResource.json";

        [SetUp]
        public void Setup()
        {
            _jSchemaResolver = new OicSchemaResolver();
            
            // Preload our resolver with desired schemas
            _jSchemaResolver.Add(OicCoreSchemaPath);
            _jSchemaResolver.Add(OicCoreSchema2Path);
            _jSchemaResolver.Add(OicBaseResourceSchemaPath);
        }

        [Test]
        public void SimpleOperationPerformsOkay()
        {
            // Arrange
            var generator = new ResourceTypeGenerator(_jSchemaResolver);
            var audioResourceSchema = @"Schemas\oic.r.audio.json";
            var audioResourceCode = @"Output\Audio.cs";

            // Act - Test oic core schema by loading base-resource schema
            TestDelegate action =
                () => generator.With(audioResourceSchema)
                               .UseNamespace("OICNet.ResourceTypes")
                               .UseAlias("rt", "ResourceTypes")
                               .UseAlias("if", "Interfaces")
                               .UseAlias("n", "Name")
                               .SubType<IOicResource>()
                               .Generate(audioResourceCode);

            // Assert

            Assert.DoesNotThrow(action);
        }
    }
}
