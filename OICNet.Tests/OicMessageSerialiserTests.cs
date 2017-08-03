using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Moq;

namespace OICNet.Tests
{
    [TestFixture]
    public class OicMessageSerialiserTests
    {
        private static IResourceTypeResolver _resolver;

        [SetUp]
        public static void Setup()
        {
            var mockResolver = new Mock<IResourceTypeResolver>();

            mockResolver
                .Setup(c => c.GetResourseType("oic.r.core"))
                .Returns(typeof(OicCoreResource));

            _resolver = mockResolver.Object;
        }

        [Test]
        public void DeserialiseJsonOicResourceCore()
        {
            // Arrange
            var serialiser = new OicMessageSerialiser(_resolver);
            var input = Encoding.UTF8.GetBytes(@"{""rt"":[""oic.r.core""],""if"":[""oic.if.baseline""],""n"":""Test"",""id"":""test""}");
            var expected = typeof(OicCoreResource);

            var actual = serialiser.Deserialise(input, OicMessageContentType.ApplicationJson).GetType();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DeserialiseCborOicResourceCore()
        {
            // Arrange
            var serialiser = new OicMessageSerialiser(_resolver);
            var input = new byte[] { 0xA4, 0x62, 0x72, 0x74, 0x81, 0x6A, 0x6F, 0x69, 0x63, 0x2E, 0x72, 0x2E, 0x63, 0x6F, 0x72, 0x65, 0x62, 0x69, 0x66, 0x81, 0x6F, 0x6F, 0x69, 0x63, 0x2E, 0x69, 0x66, 0x2E, 0x62, 0x61, 0x73, 0x65, 0x6C, 0x69, 0x6E, 0x65, 0x61, 0x6E, 0x64, 0x54, 0x65, 0x73, 0x74, 0x62, 0x69, 0x64, 0x64, 0x74, 0x65, 0x73, 0x74 };
            var expected = typeof(OicCoreResource);

            var actual = serialiser.Deserialise(input, OicMessageContentType.ApplicationCbor).GetType();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SerialiseJsonOicResourceCore()
        {
            // Arrange
            var expected = @"{""rt"":[""oic.r.core""],""if"":[""oic.if.baseline""],""n"":""Test"",""id"":""test""}";
            var serialiser = new OicMessageSerialiser(_resolver);
            var input = new OicCoreResource
            {
                Name = "Test",
                Id = "test",
                Interfaces = new List<OicResourceInterface> { OicResourceInterface.Baseline },
                ResourceTypes = new List<string> { "oic.r.core" }
            };

            var actual = serialiser.Serialise(input, OicMessageContentType.ApplicationJson);

            Assert.AreEqual(expected, Encoding.UTF8.GetString(actual));
        }

        [Test]
        public void SerialiseCborOicResourceCore()
        {
            // Arrange
            var expected = BitConverter.ToString(new byte[]{ 0xA4, 0x62, 0x72, 0x74, 0x81, 0x6A, 0x6F, 0x69, 0x63, 0x2E, 0x72, 0x2E, 0x63, 0x6F, 0x72, 0x65, 0x62, 0x69, 0x66, 0x81, 0x6F, 0x6F, 0x69, 0x63, 0x2E, 0x69, 0x66, 0x2E, 0x62, 0x61, 0x73, 0x65, 0x6C, 0x69, 0x6E, 0x65, 0x61, 0x6E, 0x64, 0x54, 0x65, 0x73, 0x74, 0x62, 0x69, 0x64, 0x64, 0x74, 0x65, 0x73, 0x74 });
            var serialiser = new OicMessageSerialiser(_resolver);
            var input = new OicCoreResource
            {
                Name = "Test",
                Id = "test",
                Interfaces = new List<OicResourceInterface> { OicResourceInterface.Baseline },
                ResourceTypes = new List<string> { "oic.r.core" }
            };

            var actual = serialiser.Serialise(input, OicMessageContentType.ApplicationCbor);

            Assert.AreEqual(expected, BitConverter.ToString(actual));
        }
    }
}
