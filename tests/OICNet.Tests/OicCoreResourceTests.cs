using System;
using System.Collections;
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
    public class OicCoreResourceTests : TestFixtureBase
    {
        [Test]
        [TestCaseSource(typeof(TestCases), nameof(TestCases.CborSerialiseTestCases))]
        public byte[] CborSerialiseObject(IOicResource input)
        {
            using (var stream = new MemoryStream())
            {
                var writer = new CborDataWriter(stream);
                JsonSerializer.CreateDefault().Serialize(writer, input);
                writer.Flush();
                return stream.ToArray();
            }
        }

        [Test]
        [TestCaseSource(typeof(TestCases), nameof(TestCases.CborDeserialiseTestCases))]
        public IOicResource CborDeserialiseObject(byte[] data, Type type)
        {
            using (var stream = new MemoryStream(data))
            {
                var reader = new CborDataReader(stream);
                return (IOicResource) JsonSerializer.CreateDefault().Deserialize(reader, type);
            }
        }

        [Test]
        [TestCaseSource(typeof(TestCases), nameof(TestCases.JsonSerialiseTestCases))]
        public string JsonSerialiseObject(IOicResource input)
        {
            using (var stringWriter = new StringWriter())
            {
                var writer = new JsonTextWriter(stringWriter);
                JsonSerializer.CreateDefault().Serialize(writer, input);
                writer.Flush();
                return stringWriter.ToString();
            }
        }

        [Test]
        [TestCaseSource(typeof(TestCases), nameof(TestCases.JsonDeserialiseTestCases))]
        public IOicResource JsonDeserialiseObject(string input, Type type)
        {
            using (var stream = new StringReader(input))
            {
                var reader = new JsonTextReader(stream);
                return (IOicResource) JsonSerializer.CreateDefault().Deserialize(reader, type);
            }
        }
    }

    public class TestCases : TestFixtureBase
    {
        public static IEnumerable CborDeserialiseTestCases
        {
            get
            {
                yield return new TestCaseData(new byte[]
                    {
                        0xA6, 0x62, 0x72, 0x74, 0x81, 0x69, 0x6F, 0x69, 0x63, 0x2E, 0x72, 0x2E, 0x66, 0x6F, 0x6F, 0x62,
                        0x69, 0x66, 0x81, 0x6F, 0x6F, 0x69, 0x63, 0x2E, 0x69, 0x66, 0x2E, 0x62, 0x61, 0x73, 0x65, 0x6C,
                        0x69, 0x6E, 0x65, 0x61, 0x6E, 0x63, 0x46, 0x6F, 0x6F, 0x62, 0x69, 0x64, 0x63, 0x66, 0x6F, 0x6F,
                        0x65, 0x76, 0x61, 0x6C, 0x75, 0x65, 0xFA, 0x3F, 0x00, 0x00, 0x00, 0x65, 0x72, 0x61, 0x6E, 0x67,
                        0x65, 0x82, 0xFA, 0x00, 0x00, 0x00, 0x00, 0xFA, 0x3F, 0x80, 0x00, 0x00
                    }, typeof(OicNumberResouece))
                    .Returns(new OicNumberResouece
                    {
                        Id = "foo",
                        Name = "Foo",
                        ResourceTypes = {"oic.r.foo"},
                        Interfaces = OicResourceInterface.Baseline,
                        Value = 0.5f,
                        Range = new List<float> {0.0f, 1.0f}
                    });

                yield return new TestCaseData(new byte[]
                    {
                        0xA6, 0x62, 0x72, 0x74, 0x81, 0x69, 0x6F, 0x69, 0x63, 0x2E, 0x72, 0x2E, 0x66, 0x6F, 0x6F, 0x62,
                        0x69,
                        0x66, 0x81, 0x6F, 0x6F, 0x69, 0x63, 0x2E, 0x69, 0x66, 0x2E, 0x62, 0x61, 0x73, 0x65, 0x6C, 0x69,
                        0x6E,
                        0x65, 0x61, 0x6E, 0x63, 0x46, 0x6F, 0x6F, 0x62, 0x69, 0x64, 0x63, 0x66, 0x6F, 0x6F, 0x65, 0x76,
                        0x61,
                        0x6C, 0x75, 0x65, 0x18, 0x32, 0x65, 0x72, 0x61, 0x6E, 0x67, 0x65, 0x82, 0x00, 0x18, 0x64
                    }, typeof(OicIntResouece))
                    .Returns(new OicIntResouece
                    {
                        Id = "foo",
                        Name = "Foo",
                        ResourceTypes =  {"oic.r.foo"},
                        Interfaces = OicResourceInterface.Baseline,
                        Value = 50,
                        Range = new List<int> {0, 100}
                    });

                yield return new TestCaseData(new byte[]
                    {
                        0xA5, 0x62, 0x72, 0x74, 0x81, 0x69, 0x6F, 0x69, 0x63, 0x2E, 0x72, 0x2E, 0x66, 0x6F, 0x6F, 0x62,
                        0x69, 0x66, 0x81, 0x6F, 0x6F, 0x69, 0x63, 0x2E, 0x69, 0x66, 0x2E, 0x62, 0x61, 0x73, 0x65, 0x6C,
                        0x69, 0x6E, 0x65, 0x61, 0x6E, 0x63, 0x46, 0x6F, 0x6F, 0x62, 0x69, 0x64, 0x63, 0x66, 0x6F, 0x6F,
                        0x65, 0x76, 0x61, 0x6C, 0x75, 0x65, 0x6B, 0x54, 0x65, 0x73, 0x74, 0x20, 0x53, 0x74, 0x72, 0x69,
                        0x6E, 0x67
                    }, typeof(OicBaseResouece<string>))
                    .Returns(new OicBaseResouece<string>
                    {
                        Id = "foo",
                        Name = "Foo",
                        ResourceTypes = {"oic.r.foo"},
                        Interfaces = OicResourceInterface.Baseline,
                        Value = "Test String"
                    });

                yield return new TestCaseData(new byte[]
                {
                    0xA4, 0x62, 0x72, 0x74, 0x81, 0x6A, 0x6F, 0x69, 0x63, 0x2E, 0x72, 0x2E, 0x74, 0x65, 0x73, 0x74,
                    0x62, 0x69, 0x66, 0x81, 0x6F, 0x6F, 0x69, 0x63, 0x2E, 0x69, 0x66, 0x2E, 0x62, 0x61, 0x73, 0x65,
                    0x6C, 0x69, 0x6E, 0x65, 0x61, 0x6E, 0x6A, 0x54, 0x65, 0x73, 0x74, 0x20, 0x41, 0x73, 0x73, 0x65,
                    0x74, 0x62, 0x69, 0x64, 0x64, 0x74, 0x65, 0x73, 0x74
                }, typeof(OicCoreResource)).Returns(new OicCoreResource
                {
                    Id = "test",
                    Name = "Test Asset",
                    ResourceTypes = 
                    {
                        "oic.r.test"
                    },
                    Interfaces = OicResourceInterface.Baseline
                });
            }
        }

        public static IEnumerable CborSerialiseTestCases
        {
            get
            {
                yield return new TestCaseData(new OicNumberResouece
                {
                    Id = "foo",
                    Name = "Foo",
                    ResourceTypes =  {"oic.r.foo"},
                    Interfaces = OicResourceInterface.Baseline,
                    Value = 0.5f,
                    Range = new List<float> {0.0f, 1.0f}
                }).Returns(new byte[]
                {
                    0xA6, 0x62, 0x72, 0x74, 0x81, 0x69, 0x6F, 0x69, 0x63, 0x2E, 0x72, 0x2E, 0x66, 0x6F, 0x6F, 0x62,
                    0x69, 0x66, 0x81, 0x6F, 0x6F, 0x69, 0x63, 0x2E, 0x69, 0x66, 0x2E, 0x62, 0x61, 0x73, 0x65, 0x6C,
                    0x69, 0x6E, 0x65, 0x61, 0x6E, 0x63, 0x46, 0x6F, 0x6F, 0x62, 0x69, 0x64, 0x63, 0x66, 0x6F, 0x6F,
                    0x65, 0x76, 0x61, 0x6C, 0x75, 0x65, 0xFA, 0x3F, 0x00, 0x00, 0x00, 0x65, 0x72, 0x61, 0x6E, 0x67,
                    0x65, 0x82, 0xFA, 0x00, 0x00, 0x00, 0x00, 0xFA, 0x3F, 0x80, 0x00, 0x00
                });
                yield return new TestCaseData(new OicIntResouece
                {
                    Id = "foo",
                    Name = "Foo",
                    ResourceTypes =  {"oic.r.foo"},
                    Interfaces = OicResourceInterface.Baseline,
                    Value = 50,
                    Range = new List<int> {0, 100}
                }).Returns(new byte[]
                {
                    0xA6, 0x62, 0x72, 0x74, 0x81, 0x69, 0x6F, 0x69, 0x63, 0x2E, 0x72, 0x2E, 0x66, 0x6F, 0x6F, 0x62,
                    0x69, 0x66, 0x81, 0x6F, 0x6F, 0x69, 0x63, 0x2E, 0x69, 0x66, 0x2E, 0x62, 0x61, 0x73, 0x65, 0x6C,
                    0x69, 0x6E, 0x65, 0x61, 0x6E, 0x63, 0x46, 0x6F, 0x6F, 0x62, 0x69, 0x64, 0x63, 0x66, 0x6F, 0x6F,
                    0x65, 0x76, 0x61, 0x6C, 0x75, 0x65, 0x18, 0x32, 0x65, 0x72, 0x61, 0x6E, 0x67, 0x65, 0x82, 0x00,
                    0x18, 0x64
                });

                yield return new TestCaseData(new OicBaseResouece<string>
                {
                    Id = "foo",
                    Name = "Foo",
                    ResourceTypes =  {"oic.r.foo"},
                    Interfaces = OicResourceInterface.Baseline,
                    Value = "Test String"
                }).Returns(new byte[]
                {
                    0xA5, 0x62, 0x72, 0x74, 0x81, 0x69, 0x6F, 0x69, 0x63, 0x2E, 0x72, 0x2E, 0x66, 0x6F, 0x6F, 0x62,
                    0x69, 0x66, 0x81, 0x6F, 0x6F, 0x69, 0x63, 0x2E, 0x69, 0x66, 0x2E, 0x62, 0x61, 0x73, 0x65, 0x6C,
                    0x69, 0x6E, 0x65, 0x61, 0x6E, 0x63, 0x46, 0x6F, 0x6F, 0x62, 0x69, 0x64, 0x63, 0x66, 0x6F, 0x6F,
                    0x65, 0x76, 0x61, 0x6C, 0x75, 0x65, 0x6B, 0x54, 0x65, 0x73, 0x74, 0x20, 0x53, 0x74, 0x72, 0x69,
                    0x6E, 0x67
                });

                yield return new TestCaseData(new OicCoreResource
                {
                    Id = "test",
                    Name = "Test Asset",
                    ResourceTypes = 
                    {
                        "oic.r.test"
                    },
                    Interfaces = OicResourceInterface.Baseline
                }).Returns(new byte[]
                {
                    0xA4, 0x62, 0x72, 0x74, 0x81, 0x6A, 0x6F, 0x69, 0x63, 0x2E, 0x72, 0x2E, 0x74, 0x65, 0x73, 0x74,
                    0x62, 0x69, 0x66, 0x81, 0x6F, 0x6F, 0x69, 0x63, 0x2E, 0x69, 0x66, 0x2E, 0x62, 0x61, 0x73, 0x65,
                    0x6C, 0x69, 0x6E, 0x65, 0x61, 0x6E, 0x6A, 0x54, 0x65, 0x73, 0x74, 0x20, 0x41, 0x73, 0x73, 0x65,
                    0x74, 0x62, 0x69, 0x64, 0x64, 0x74, 0x65, 0x73, 0x74
                });
            }
        }

        public static IEnumerable JsonSerialiseTestCases
        {
            get
            {
                yield return new TestCaseData(new OicNumberResouece
                {
                    Id = "foo",
                    Name = "Foo",
                    ResourceTypes =  {"oic.r.foo"},
                    Interfaces = OicResourceInterface.Baseline,
                    Value = 0.5f,
                    Range = new List<float> {0.0f, 1.0f}
                }).Returns(
                    "{\"rt\":[\"oic.r.foo\"],\"if\":[\"oic.if.baseline\"],\"n\":\"Foo\",\"id\":\"foo\",\"value\":0.5,\"range\":[0.0,1.0]}");

                yield return new TestCaseData(new OicIntResouece
                {
                    Id = "foo",
                    Name = "Foo",
                    ResourceTypes =  {"oic.r.foo"},
                    Interfaces = OicResourceInterface.Baseline,
                    Value = 50,
                    Range = new List<int> {0, 100}
                }).Returns(
                    "{\"rt\":[\"oic.r.foo\"],\"if\":[\"oic.if.baseline\"],\"n\":\"Foo\",\"id\":\"foo\",\"value\":50,\"range\":[0,100]}");

                yield return new TestCaseData(new OicBaseResouece<string>
                {
                    Id = "foo",
                    Name = "Foo",
                    ResourceTypes =  {"oic.r.foo"},
                    Interfaces = OicResourceInterface.Baseline,
                    Value = "Test String"
                }).Returns(
                    "{\"rt\":[\"oic.r.foo\"],\"if\":[\"oic.if.baseline\"],\"n\":\"Foo\",\"id\":\"foo\",\"value\":\"Test String\"}");

                yield return new TestCaseData(new OicCoreResource
                {
                    Id = "test",
                    Name = "Test Asset",
                    ResourceTypes = 
                    {
                        "oic.r.test"
                    },
                    Interfaces = OicResourceInterface.Baseline
                }).Returns(
                    "{\"rt\":[\"oic.r.test\"],\"if\":[\"oic.if.baseline\"],\"n\":\"Test Asset\",\"id\":\"test\"}");
            }
        }

        public static IEnumerable JsonDeserialiseTestCases
        {
            get
            {
                yield return new TestCaseData(
                        "{\"rt\":[\"oic.r.foo\"],\"if\":[\"oic.if.baseline\"],\"n\":\"Foo\",\"id\":\"foo\",\"value\":0.5,\"range\":[0.0,1.0]}",
                        typeof(OicNumberResouece))
                    .Returns(new OicNumberResouece
                    {
                        Id = "foo",
                        Name = "Foo",
                        ResourceTypes =  {"oic.r.foo"},
                        Interfaces = OicResourceInterface.Baseline,
                        Value = 0.5f,
                        Range = new List<float> {0.0f, 1.0f}
                    });

                yield return new TestCaseData(
                        "{\"rt\":[\"oic.r.foo\"],\"if\":[\"oic.if.baseline\"],\"n\":\"Foo\",\"id\":\"foo\",\"value\":50,\"range\":[0,100]}",
                        typeof(OicIntResouece))
                    .Returns(new OicIntResouece
                    {
                        Id = "foo",
                        Name = "Foo",
                        ResourceTypes =  {"oic.r.foo"},
                        Interfaces = OicResourceInterface.Baseline,
                        Value = 50,
                        Range = new List<int> {0, 100}
                    });

                yield return new TestCaseData(
                        "{\"rt\":[\"oic.r.foo\"],\"if\":[\"oic.if.baseline\"],\"n\":\"Foo\",\"id\":\"foo\",\"value\":\"Test String\"}",
                        typeof(OicBaseResouece<string>))
                    .Returns(new OicBaseResouece<string>
                    {
                        Id = "foo",
                        Name = "Foo",
                        ResourceTypes =  {"oic.r.foo"},
                        Interfaces = OicResourceInterface.Baseline,
                        Value = "Test String"
                    });

                yield return new TestCaseData(
                        "{\"rt\":[\"oic.r.test\"],\"if\":[\"oic.if.baseline\"],\"n\":\"Test Asset\",\"id\":\"test\"}",
                        typeof(OicCoreResource))
                    .Returns(new OicCoreResource
                    {
                        Id = "test",
                        Name = "Test Asset",
                        ResourceTypes = 
                        {
                            "oic.r.test"
                        },
                        Interfaces = OicResourceInterface.Baseline
                    });
            }
        }
    }
}
