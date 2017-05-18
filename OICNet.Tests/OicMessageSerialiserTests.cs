using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

using Moq;

namespace OICNet.Tests
{
    [TestClass]
    public class OicMessageSerialiserTests
    {
        private static IResourceTypeResolver _resolver;

        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            var mockResolver = new Mock<IResourceTypeResolver>();

            mockResolver
                .Setup(c => c.GetResourseType("oic.r.core"))
                .Returns(typeof(OicCoreResource));

            _resolver = mockResolver.Object;
        }

        [TestMethod]
        public void DeserialiseOicResourceCore()
        {
            // Arrange
            var serialiser = new OicMessageSerialiser(_resolver);
            var input = "{\"rt\":[\"oic.r.core\"],\"if\":[\"oic.if.baseline\"],\"n\":\"Test\",\"id\":\"test\"}";
            var expected = typeof(OicCoreResource);

            var actual = serialiser.Deserialise(input).GetType();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SerialiseOicResourceCore()
        {
            // Arrange
            var expected = "{\"rt\":[\"oic.r.core\"],\"if\":[\"oic.if.baseline\"],\"n\":\"Test\",\"id\":\"test\"}";
            var serialiser = new OicMessageSerialiser(_resolver);
            var input = new OicCoreResource
            {
                Name = "Test",
                Id = "test",
                Interfaces = new List<OicResourceInterface> { OicResourceInterface.Baseline },
                ResourceTypes = new List<string> { "oic.r.core" }
            };

            var actual = serialiser.Serialise(input);

            Assert.AreEqual(expected, actual);
        }
    }
}
