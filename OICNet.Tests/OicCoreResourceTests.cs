using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using OICNet;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace OICNet.Tests
{
    [TestClass]
    public class OicCoreResourceTests
    {
        [TestMethod]
        public void CoreFromJson()
        {
            // Arrange 
            var input = "{\"rt\":[\"oic.r.test\"],\"if\":[\"oic.if.baseline\"],\"n\":\"Test Asset\",\"id\":\"test\"}";

            var expected = new OicCoreResource
            {
                Id = "test",
                Name = "Test Asset",
                ResourceTypes = new List<string> { "oic.r.test" },
                Interfaces = new List<OicResourceInterface> { OicResourceInterface.Baseline }
            };

            // Act
            var actual = JsonConvert.DeserializeObject<OicCoreResource>(input);

            // Assert
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.IsTrue(expected.Interfaces.SequenceEqual(actual.Interfaces));
            Assert.IsTrue(expected.ResourceTypes.SequenceEqual(actual.ResourceTypes));
        }

        [TestMethod]
        public void CoreToJson()
        {
            var input = new OicCoreResource
            {
                Id = "test",
                Name = "Test Asset",
                ResourceTypes = new List<string>
                {
                    "oic.r.test"
                },
                Interfaces = new List<OicResourceInterface>
                {
                    OicResourceInterface.Baseline
                }
            };

            var expected = "{\"rt\":[\"oic.r.test\"],\"if\":[\"oic.if.baseline\"],\"n\":\"Test Asset\",\"id\":\"test\"}";

            var actual = JsonConvert.SerializeObject(input);

            //System.Diagnostics.Debug.WriteLine($"Result => {actual}");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CoreFromCbor()
        {
            Assert.Inconclusive("Not Inplemented");
        }

        [TestMethod]
        public void CoreToCbor()
        {
            Assert.Inconclusive("Not Inplemented");
        }

        [TestMethod]
        public void StringResourceToJson()
        {
            var foo = new OicBaseResouece<string>
            {
                Id = "foo",
                Name = "Foo",
                ResourceTypes = new List<string>{ "oic.r.foo" },
                Interfaces = new List<OicResourceInterface>{ OicResourceInterface.Baseline },
                Value = "Test String"
            };

            var expected = "{\"rt\":[\"oic.r.foo\"],\"if\":[\"oic.if.baseline\"],\"n\":\"Foo\",\"id\":\"foo\",\"value\":\"Test String\"}";

            var actual = JsonConvert.SerializeObject(foo);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void NumberResourceToJson()
        {
            var foo = new OicBaseResouece<int>
            {
                Id = "foo",
                Name = "Foo",
                ResourceTypes = new List<string> { "oic.r.foo" },
                Interfaces = new List<OicResourceInterface> { OicResourceInterface.Baseline },
                Value = 50,
                Range = new List<decimal> { 0, 100 }
            };

            var expected = "{\"rt\":[\"oic.r.foo\"],\"if\":[\"oic.if.baseline\"],\"n\":\"Foo\",\"id\":\"foo\",\"value\":50,\"range\":[0,100]}";

            var actual = JsonConvert.SerializeObject(foo);

            Assert.AreEqual(expected, actual);
        }
    }
}
