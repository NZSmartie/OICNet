using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Cbor;
using NUnit.Framework;

using OICNet;

namespace OICNet.Tests
{
    [TestFixture]
    public class OicCoreResourceTests
    {
        [Test]
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

        [Test]
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

        [Test]
        public void CoreFromCbor()
        {
            // Arrange 
            var input = ToBytes("A4-62-72-74-81-6A-6F-69-63-2E-72-2E-74-65-73-74-62-69-66-81-6F-6F-69-63-2E-69-66-2E-62-61-73-65-6C-69-6E-65-61-6E-6A-54-65-73-74-20-41-73-73-65-74-62-69-64-64-74-65-73-74");

            var expected = new OicCoreResource
            {
                Id = "test",
                Name = "Test Asset",
                ResourceTypes = new List<string> { "oic.r.test" },
                Interfaces = new List<OicResourceInterface> { OicResourceInterface.Baseline }
            };

            // Act
            var actual = CborDeserialiseObject<OicCoreResource>(input);

            // Assert
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.IsTrue(expected.Interfaces.SequenceEqual(actual.Interfaces));
            Assert.IsTrue(expected.ResourceTypes.SequenceEqual(actual.ResourceTypes));
        }

        [Test]
        public void CoreToCbor()
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

            var expected = "A4-62-72-74-81-6A-6F-69-63-2E-72-2E-74-65-73-74-62-69-66-81-6F-6F-69-63-2E-69-66-2E-62-61-73-65-6C-69-6E-65-61-6E-6A-54-65-73-74-20-41-73-73-65-74-62-69-64-64-74-65-73-74";

            var actual = CborSerialiseObject(input);

            //System.Diagnostics.Debug.WriteLine($"Result => {actual}");
            Assert.AreEqual(expected, BitConverter.ToString(actual));
        }

        [Test]
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

        [Test]
        public void StringResourceToCbor()
        {
            var foo = new OicBaseResouece<string>
            {
                Id = "foo",
                Name = "Foo",
                ResourceTypes = new List<string> { "oic.r.foo" },
                Interfaces = new List<OicResourceInterface> { OicResourceInterface.Baseline },
                Value = "Test String"
            };

            var expected = "A5-62-72-74-81-69-6F-69-63-2E-72-2E-66-6F-6F-62-69-66-81-6F-6F-69-63-2E-69-66-2E-62-61-73-65-6C-69-6E-65-61-6E-63-46-6F-6F-62-69-64-63-66-6F-6F-65-76-61-6C-75-65-6B-54-65-73-74-20-53-74-72-69-6E-67";

            var actual = BitConverter.ToString(CborSerialiseObject(foo));

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void IntegerResourceToJson()
        {
            var foo = new OicIntResouece
            {
                Id = "foo",
                Name = "Foo",
                ResourceTypes = new List<string> { "oic.r.foo" },
                Interfaces = new List<OicResourceInterface> { OicResourceInterface.Baseline },
                Value = 50,
                Range = new List<int> { 0, 100 }
            };

            var expected = "{\"rt\":[\"oic.r.foo\"],\"if\":[\"oic.if.baseline\"],\"n\":\"Foo\",\"id\":\"foo\",\"value\":50,\"range\":[0,100]}";

            var actual = JsonConvert.SerializeObject(foo);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void IntegerResourceToCbor()
        {
            var foo = new OicIntResouece
            {
                Id = "foo",
                Name = "Foo",
                ResourceTypes = new List<string> { "oic.r.foo" },
                Interfaces = new List<OicResourceInterface> { OicResourceInterface.Baseline },
                Value = 50,
                Range = new List<int> { 0, 100 }
            };

            var expected = "A6-62-72-74-81-69-6F-69-63-2E-72-2E-66-6F-6F-62-69-66-81-6F-6F-69-63-2E-69-66-2E-62-61-73-65-6C-69-6E-65-61-6E-63-46-6F-6F-62-69-64-63-66-6F-6F-65-76-61-6C-75-65-18-32-65-72-61-6E-67-65-82-00-18-64";

            var actual = BitConverter.ToString(CborSerialiseObject(foo));

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void NumberResourceToJson()
        {
            var foo = new OicNumberResouece
            {
                Id = "foo",
                Name = "Foo",
                ResourceTypes = new List<string> { "oic.r.foo" },
                Interfaces = new List<OicResourceInterface> { OicResourceInterface.Baseline },
                Value = 0.5f,
                Range = new List<float> { 0.0f, 1.0f }
            };

            var expected = "{\"rt\":[\"oic.r.foo\"],\"if\":[\"oic.if.baseline\"],\"n\":\"Foo\",\"id\":\"foo\",\"value\":0.5,\"range\":[0.0,1.0]}";

            var actual = JsonConvert.SerializeObject(foo);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void NumberResourceToCbor()
        {
            var foo = new OicNumberResouece
            {
                Id = "foo",
                Name = "Foo",
                ResourceTypes = new List<string> { "oic.r.foo" },
                Interfaces = new List<OicResourceInterface> { OicResourceInterface.Baseline },
                Value = 0.5f,
                Range = new List<float> { 0.0f, 1.0f }
            };

            var expected = "A6-62-72-74-81-69-6F-69-63-2E-72-2E-66-6F-6F-62-69-66-81-6F-6F-69-63-2E-69-66-2E-62-61-73-65-6C-69-6E-65-61-6E-63-46-6F-6F-62-69-64-63-66-6F-6F-65-76-61-6C-75-65-FA-3F-00-00-00-65-72-61-6E-67-65-82-FA-00-00-00-00-FA-3F-80-00-00";

            var actual = BitConverter.ToString(CborSerialiseObject(foo));

            Assert.AreEqual(expected, actual);
        }

        private static byte[] CborSerialiseObject(object value)
        {
            using (var stream = new MemoryStream())
            {
                var writer = new CborDataWriter(stream);
                JsonSerializer.CreateDefault().Serialize(writer, value);
                writer.Flush();
                return stream.ToArray();
            }
        }

        private static TValue CborDeserialiseObject<TValue>(byte[] data)
        {
            using (var stream = new MemoryStream(data))
            {
                var reader = new CborDataReader(stream);
                return JsonSerializer.CreateDefault().Deserialize<TValue>(reader);
            }
        }

        private static byte[] ToBytes(string value)
        {
            return value.Split('-').Select(b => Convert.ToByte(b, 16)).ToArray();
        }
    }
}
