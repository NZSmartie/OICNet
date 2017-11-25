using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using NUnit.Framework;
using System;
using System.Linq;
using System.IO;
using System.Reflection;

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
        public void Generate_KnownAudioResourceType()
        {
            // Arrange
            var generator = new ResourceTypeGenerator(_jSchemaResolver);
            var audioResourceSchema = @"Schemas\oic.r.audio.json";

            // Act - Test oic core schema by loading base-resource schema
            var audioAssembly = generator.With(audioResourceSchema)
                               .UseNamespace("OICNet.ResourceTypes")
                               .UseAlias("rt", "ResourceTypes")
                               .UseAlias("if", "Interfaces")
                               .UseAlias("n", "Name")
                               .Generate();

            // Assert

            // Throws exception if OicRAudio does not exist
            var audioType = audioAssembly.GetType("OicRAudio", true);

            var volumeProperty = audioType.GetProperty("Volume");
            Assert.That(volumeProperty, Is.Not.Null, "OicRAudio should have a property named Volume");
            Assert.That(volumeProperty.PropertyType, Is.EqualTo(typeof(int)), "OicRAudio.Volume should be an int type");
            Assert.That(volumeProperty.CustomAttributes.Count(ca => ca.AttributeType == typeof(JsonPropertyAttribute)), Is.EqualTo(1), "OicRAudio.Volume should have just one JsonPropertyAttribute");

            var jsonAttribute = volumeProperty.GetCustomAttribute<JsonPropertyAttribute>();
            Assert.That(jsonAttribute.PropertyName, Is.EqualTo("volume"), "OicRAudio.Volume should have JsonPropertyAttribute.PropertyName set to \"volume\"");
            Assert.That(jsonAttribute.Required, Is.EqualTo(Required.Always), "OicRAudio.Volume should have JsonPropertyAttribute.Requried set to Required.Always");
        }

            [Test]
        public void GenerateAndAutoSubclass_KnownAudioResourceType()
        {
            // Arrange
            var generator = new ResourceTypeGenerator(_jSchemaResolver);
            var audioResourceSchema = @"Schemas\oic.r.audio.json";

            // Act - Test oic core schema by loading base-resource schema
            var audioAssembly = generator.With(audioResourceSchema)
                               .UseNamespace("OICNet.ResourceTypes")
                               .UseAlias("rt", "ResourceTypes")
                               .UseAlias("if", "Interfaces")
                               .UseAlias("n", "Name")
                               .SubType<OicCoreResource>()
                               .SubType<CoreResources.OicResourceLink>()
                               .Generate();

            // Assert

            // Throws exception if OicRAudio does not exist
            var audioType = audioAssembly.GetType("OicRAudio", true);

            var volumeProperty = audioType.GetProperty("Volume");
            Assert.That(volumeProperty, Is.Not.Null, "OicRAudio should have a property named Volume");
            Assert.That(volumeProperty.PropertyType, Is.EqualTo(typeof(int)), "OicRAudio.Volume should be an int type");
            Assert.That(volumeProperty.CustomAttributes.Count(ca => ca.AttributeType == typeof(JsonPropertyAttribute)), Is.EqualTo(1), "OicRAudio.Volume should have just one JsonPropertyAttribute");

            var jsonAttribute = volumeProperty.GetCustomAttribute<JsonPropertyAttribute>();
            Assert.That(jsonAttribute.PropertyName, Is.EqualTo("volume"), "OicRAudio.Volume should have JsonPropertyAttribute.PropertyName set to \"volume\"");
            Assert.That(jsonAttribute.Required, Is.EqualTo(Required.Always), "OicRAudio.Volume should have JsonPropertyAttribute.Requried set to Required.Always");
        }
    }
}
