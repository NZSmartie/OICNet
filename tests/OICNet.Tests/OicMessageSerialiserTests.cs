using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Moq;

namespace OICNet.Tests
{
    [TestFixture]
    public class OicMessageSerialiserTests
    {
        private static ResourceTypeResolver _resolver;

        [SetUp]
        public static void Setup()
        {
            var mockResolver = new Mock<ResourceTypeResolver> {CallBase = true};

            var type = typeof(OicCoreResource);
            mockResolver.Setup(c => c.TryGetResourseType("oic.r.core", out type)).Returns(true);
            type = typeof(ResourceTypes.Audio);
            mockResolver.Setup(c => c.TryGetResourseType("oic.r.audio", out type)).Returns(true);
            type = typeof(CoreResources.OicResourceDirectory);
            mockResolver.Setup(c => c.TryGetResourseType("oic.wk.res", out type)).Returns(true);

            _resolver = mockResolver.Object;
        }

        [Test]
        public void DeserialiseMissingResourceType()
        {
            Assert.Throws<InvalidDataException>(() =>
            {
                // Arrange
                var serialiser = new OicMessageSerialiser(_resolver);

                var input = Encoding.UTF8.GetBytes(
                    "{\"if\":[\"oic.if.baseline\"],\"n\":\"Test\",\"id\":\"test\"}");

                //Only worried about the first result
                serialiser.Deserialise(input, OicMessageContentType.ApplicationJson).ToList();
            });

            Assert.Throws<InvalidDataException>(() =>
            {
                // Arrange
                var serialiser = new OicMessageSerialiser(_resolver);

                var input = Encoding.UTF8.GetBytes(
                    "[{\"if\":[\"oic.if.baseline\"],\"n\":\"Test\",\"id\":\"test\"}]");

                //Only worried about the first result
                serialiser.Deserialise(input, OicMessageContentType.ApplicationJson).ToList();
            });
        }

        [Test, TestCaseSource(typeof(SerialiserTestCaseData), nameof(SerialiserTestCaseData.SerialiseTestCases))]
        public byte[] SerialiseOicResourceCore(IOicResource input, OicMessageContentType type)
        {
            // Arrange
            var serialiser = new OicMessageSerialiser(_resolver);
            return serialiser.Serialise(input, type);
        }

        [Test, TestCaseSource(typeof(SerialiserTestCaseData), nameof(SerialiserTestCaseData.DeserialiseTestCases))]
        public IOicResource DeserialiseOicResourceCore(byte[] input, OicMessageContentType type)
        {
            // Arrange
            var serialiser = new OicMessageSerialiser(_resolver);

            //Only worried about the first result
            return serialiser.Deserialise(input, type).First();
        }

        [Test, TestCaseSource(typeof(SerialiserTestCaseData), nameof(SerialiserTestCaseData.DeserialiseArrayTestCases))]
        public IList<IOicResource> DeserialiseOicResourceCoreArray(byte[] input, OicMessageContentType type)
        {
            // Arrange
            var serialiser = new OicMessageSerialiser(_resolver);
            return serialiser.Deserialise(input, type).ToList();
        }
    }

    public class SerialiserTestCaseData
    {
        public static IEnumerable SerialiseTestCases
        {
            get
            {
                yield return new TestCaseData(new OicCoreResource
                {
                    Name = "Test",
                    Id = "test",
                    Interfaces =  OicResourceInterface.Baseline,
                    ResourceTypes =  {"oic.r.core"}
                }, OicMessageContentType.ApplicationJson).Returns(Encoding.UTF8.GetBytes(
                    "{\"rt\":[\"oic.r.core\"],\"if\":[\"oic.if.baseline\"],\"n\":\"Test\",\"id\":\"test\"}"));

                yield return new TestCaseData(new ResourceTypes.Audio()
                    {
                        Name = "AudioTest",
                        Id = "audio-test",
                        Mute = false,
                        Volume = 75
                    }, OicMessageContentType.ApplicationJson)
                    .Returns(Encoding.UTF8.GetBytes(
                        "{\"rt\":[\"oic.r.audio\"],\"if\":[\"oic.if.a\",\"oic.if.baseline\"],\"n\":\"AudioTest\",\"id\":\"audio-test\",\"volume\":75,\"mute\":false}"));

                yield return new TestCaseData(new OicCoreResource
                {
                    Name = "Test",
                    Id = "test",
                    Interfaces = OicResourceInterface.Baseline,
                    ResourceTypes = {"oic.r.core"}
                }, OicMessageContentType.ApplicationCbor).Returns(new byte[]
                {
                    0xA4, 0x62, 0x72, 0x74, 0x81, 0x6A, 0x6F, 0x69, 0x63, 0x2E, 0x72, 0x2E, 0x63, 0x6F, 0x72, 0x65,
                    0x62, 0x69, 0x66, 0x81, 0x6F, 0x6F, 0x69, 0x63, 0x2E, 0x69, 0x66, 0x2E, 0x62, 0x61, 0x73, 0x65,
                    0x6C, 0x69, 0x6E, 0x65, 0x61, 0x6E, 0x64, 0x54, 0x65, 0x73, 0x74, 0x62, 0x69, 0x64, 0x64, 0x74,
                    0x65, 0x73, 0x74
                });

                yield return new TestCaseData(new ResourceTypes.Audio()
                    {
                        Name = "AudioTest",
                        Id = "audio-test",
                        Mute = false,
                        Volume = 75
                    }, OicMessageContentType.ApplicationCbor)
                    .Returns(new byte[]
                    {
                        0xa6, 0x62, 0x72, 0x74, 0x81, 0x6b, 0x6f, 0x69, 0x63, 0x2e, 0x72, 0x2e, 0x61, 0x75, 0x64, 0x69,
                        0x6f, 0x62, 0x69, 0x66, 0x82, 0x68, 0x6f, 0x69, 0x63, 0x2e, 0x69, 0x66, 0x2e, 0x61, 0x6f, 0x6f,
                        0x69, 0x63, 0x2e, 0x69, 0x66, 0x2e, 0x62, 0x61, 0x73, 0x65, 0x6c, 0x69, 0x6e, 0x65, 0x61, 0x6e,
                        0x69, 0x41, 0x75, 0x64, 0x69, 0x6f, 0x54, 0x65, 0x73, 0x74, 0x62, 0x69, 0x64, 0x6a, 0x61, 0x75,
                        0x64, 0x69, 0x6f, 0x2d, 0x74, 0x65, 0x73, 0x74, 0x66, 0x76, 0x6f, 0x6c, 0x75, 0x6d, 0x65, 0x18,
                        0x4b, 0x64, 0x6d, 0x75, 0x74, 0x65, 0xf4
                    });
            }
        }

        public static IEnumerable DeserialiseTestCases
        {
            get
            {
                yield return new TestCaseData(
                        Encoding.UTF8.GetBytes(
                            "{\"rt\":[\"oic.r.core\"],\"if\":[\"oic.if.baseline\"],\"n\":\"Test\",\"id\":\"test\"}"),
                        OicMessageContentType.ApplicationJson)
                    .Returns(new OicCoreResource
                    {
                        Name = "Test",
                        Id = "test",
                        Interfaces = OicResourceInterface.Baseline,
                        ResourceTypes =  {"oic.r.core"}
                    });

                yield return new TestCaseData(new byte[]
                    {
                        0xA4, 0x62, 0x72, 0x74, 0x81, 0x6A, 0x6F, 0x69, 0x63, 0x2E, 0x72, 0x2E, 0x63, 0x6F, 0x72, 0x65,
                        0x62, 0x69, 0x66, 0x81, 0x6F, 0x6F, 0x69, 0x63, 0x2E, 0x69, 0x66, 0x2E, 0x62, 0x61, 0x73, 0x65,
                        0x6C, 0x69, 0x6E, 0x65, 0x61, 0x6E, 0x64, 0x54, 0x65, 0x73, 0x74, 0x62, 0x69, 0x64, 0x64, 0x74,
                        0x65, 0x73, 0x74
                    }, OicMessageContentType.ApplicationCbor)
                    .Returns(new OicCoreResource
                    {
                        Name = "Test",
                        Id = "test",
                        Interfaces = OicResourceInterface.Baseline,
                        ResourceTypes =  {"oic.r.core"}
                    });

                yield return new TestCaseData(
                        Encoding.UTF8.GetBytes(
                            "{\"rt\":[\"oic.r.audio\"],\"if\":[\"oic.if.baseline\"],\"n\":\"AudioTest\",\"id\":\"audio-test\",\"volume\":75,\"mute\":false}"),
                        OicMessageContentType.ApplicationJson)
                    .Returns(new ResourceTypes.Audio()
                    {
                        Name = "AudioTest",
                        Id = "audio-test",
                        Interfaces = OicResourceInterface.Baseline,
                        ResourceTypes =  {"oic.r.audio"},
                        Mute = false,
                        Volume = 75
                    });

                yield return new TestCaseData(new byte[]
                    {
                        0xA6, 0x62, 0x72, 0x74, 0x81, 0x6B, 0x6F, 0x69, 0x63, 0x2E, 0x72, 0x2E, 0x61, 0x75, 0x64, 0x69,
                        0x6F, 0x62, 0x69, 0x66, 0x81, 0x6F, 0x6F, 0x69, 0x63, 0x2E, 0x69, 0x66, 0x2E, 0x62, 0x61, 0x73,
                        0x65, 0x6C, 0x69, 0x6E, 0x65, 0x61, 0x6E, 0x69, 0x41, 0x75, 0x64, 0x69, 0x6F, 0x54, 0x65, 0x73,
                        0x74, 0x62, 0x69, 0x64, 0x6A, 0x61, 0x75, 0x64, 0x69, 0x6F, 0x2D, 0x74, 0x65, 0x73, 0x74, 0x66,
                        0x76, 0x6F, 0x6C, 0x75, 0x6D, 0x65, 0x18, 0x4B, 0x64, 0x6D, 0x75, 0x74, 0x65, 0xF4
                    }, OicMessageContentType.ApplicationCbor)
                    .Returns(new ResourceTypes.Audio()
                    {
                        Name = "AudioTest",
                        Id = "audio-test",
                        Interfaces = OicResourceInterface.Baseline,
                        ResourceTypes =  {"oic.r.audio"},
                        Mute = false,
                        Volume = 75
                    });
            }
        }

        public static IEnumerable DeserialiseArrayTestCases
        {
            get
            {
                yield return new TestCaseData(
                        Encoding.UTF8.GetBytes(
                            @"[{""rt"": [""oic.wk.res""],""di"": ""0685B960-736F-46F7-BEC0-9E6CBD61ADC1"",""links"":[{""href"": ""/res"",""rel"": ""self"",""rt"": [""oic.r.collection""],""if"": [""oic.if.ll""]},{""href"": ""/smartDevice"",""rel"": ""contained"",""rt"": [""oic.d.smartDevice""],""if"": [""oic.if.a""]}]},{""rt"": [""oic.wk.res""],""di"": ""0685B960-736F-46F7-BEC0-9E6CBD61ADC1"",""links"":[{""href"": ""/res"",""rel"": ""self"",""rt"": [""oic.r.collection""],""if"": [""oic.if.ll""]},{""href"": ""/smartDevice"",""rel"": ""contained"",""rt"": [""oic.d.smartDevice""],""if"": [""oic.if.a""]}]}]"),
                        OicMessageContentType.ApplicationJson)
                    .Returns(new List<IOicResource>
                    {
                        new CoreResources.OicResourceDirectory
                        {
                            ResourceTypes =  {"oic.wk.res"},
                            DeviceId = new Guid("0685B960-736F-46F7-BEC0-9E6CBD61ADC1"),
                            Links = new List<CoreResources.OicResourceLink>
                            {
                                new CoreResources.OicResourceLink
                                {
                                    Href = new Uri("/res", UriKind.Relative),
                                    Rel = "self",
                                    ResourceTypes =  {"oic.r.collection"},
                                    Interfaces = OicResourceInterface.LinkLists,
                                },
                                new CoreResources.OicResourceLink
                                {
                                    Href = new Uri("/smartDevice", UriKind.Relative),
                                    Rel = "contained",
                                    ResourceTypes =  {"oic.d.smartDevice"},
                                    Interfaces = OicResourceInterface.Actuator,
                                }
                            }
                        },
                        new CoreResources.OicResourceDirectory
                        {
                            ResourceTypes =  {"oic.wk.res"},
                            DeviceId = new Guid("0685B960-736F-46F7-BEC0-9E6CBD61ADC1"),
                            Links = new List<CoreResources.OicResourceLink>
                            {
                                new CoreResources.OicResourceLink
                                {
                                    Href = new Uri("/res", UriKind.Relative),
                                    Rel = "self",
                                    ResourceTypes =  {"oic.r.collection"},
                                    Interfaces = OicResourceInterface.LinkLists,
                                },
                                new CoreResources.OicResourceLink
                                {
                                    Href = new Uri("/smartDevice", UriKind.Relative),
                                    Rel = "contained",
                                    ResourceTypes =  {"oic.d.smartDevice"},
                                    Interfaces = OicResourceInterface.Actuator,
                                }
                            }
                        }
                    });
                yield return new TestCaseData(
                        new byte[]{ 0x82, 0xA3, 0x62, 0x72, 0x74, 0x81, 0x6A, 0x6F, 0x69, 0x63, 0x2E, 0x77, 0x6B, 0x2E, 0x72, 0x65, 0x73, 0x62, 0x64, 0x69, 0x78, 0x24, 0x30, 0x36, 0x38, 0x35, 0x42, 0x39, 0x36, 0x30, 0x2D, 0x37, 0x33, 0x36, 0x46, 0x2D, 0x34, 0x36, 0x46, 0x37, 0x2D, 0x42, 0x45, 0x43, 0x30, 0x2D, 0x39, 0x45, 0x36, 0x43, 0x42, 0x44, 0x36, 0x31, 0x41, 0x44, 0x43, 0x31, 0x65, 0x6C, 0x69, 0x6E, 0x6B, 0x73, 0x82, 0xA4, 0x64, 0x68, 0x72, 0x65, 0x66, 0x64, 0x2F, 0x72, 0x65, 0x73, 0x63, 0x72, 0x65, 0x6C, 0x64, 0x73, 0x65, 0x6C, 0x66, 0x62, 0x72, 0x74, 0x81, 0x70, 0x6F, 0x69, 0x63, 0x2E, 0x72, 0x2E, 0x63, 0x6F, 0x6C, 0x6C, 0x65, 0x63, 0x74, 0x69, 0x6F, 0x6E, 0x62, 0x69, 0x66, 0x81, 0x69, 0x6F, 0x69, 0x63, 0x2E, 0x69, 0x66, 0x2E, 0x6C, 0x6C, 0xA4, 0x64, 0x68, 0x72, 0x65, 0x66, 0x6C, 0x2F, 0x73, 0x6D, 0x61, 0x72, 0x74, 0x44, 0x65, 0x76, 0x69, 0x63, 0x65, 0x63, 0x72, 0x65, 0x6C, 0x69, 0x63, 0x6F, 0x6E, 0x74, 0x61, 0x69, 0x6E, 0x65, 0x64, 0x62, 0x72, 0x74, 0x81, 0x71, 0x6F, 0x69, 0x63, 0x2E, 0x64, 0x2E, 0x73, 0x6D, 0x61, 0x72, 0x74, 0x44, 0x65, 0x76, 0x69, 0x63, 0x65, 0x62, 0x69, 0x66, 0x81, 0x68, 0x6F, 0x69, 0x63, 0x2E, 0x69, 0x66, 0x2E, 0x61, 0xA3, 0x62, 0x72, 0x74, 0x81, 0x6A, 0x6F, 0x69, 0x63, 0x2E, 0x77, 0x6B, 0x2E, 0x72, 0x65, 0x73, 0x62, 0x64, 0x69, 0x78, 0x24, 0x30, 0x36, 0x38, 0x35, 0x42, 0x39, 0x36, 0x30, 0x2D, 0x37, 0x33, 0x36, 0x46, 0x2D, 0x34, 0x36, 0x46, 0x37, 0x2D, 0x42, 0x45, 0x43, 0x30, 0x2D, 0x39, 0x45, 0x36, 0x43, 0x42, 0x44, 0x36, 0x31, 0x41, 0x44, 0x43, 0x31, 0x65, 0x6C, 0x69, 0x6E, 0x6B, 0x73, 0x82, 0xA4, 0x64, 0x68, 0x72, 0x65, 0x66, 0x64, 0x2F, 0x72, 0x65, 0x73, 0x63, 0x72, 0x65, 0x6C, 0x64, 0x73, 0x65, 0x6C, 0x66, 0x62, 0x72, 0x74, 0x81, 0x70, 0x6F, 0x69, 0x63, 0x2E, 0x72, 0x2E, 0x63, 0x6F, 0x6C, 0x6C, 0x65, 0x63, 0x74, 0x69, 0x6F, 0x6E, 0x62, 0x69, 0x66, 0x81, 0x69, 0x6F, 0x69, 0x63, 0x2E, 0x69, 0x66, 0x2E, 0x6C, 0x6C, 0xA4, 0x64, 0x68, 0x72, 0x65, 0x66, 0x6C, 0x2F, 0x73, 0x6D, 0x61, 0x72, 0x74, 0x44, 0x65, 0x76, 0x69, 0x63, 0x65, 0x63, 0x72, 0x65, 0x6C, 0x69, 0x63, 0x6F, 0x6E, 0x74, 0x61, 0x69, 0x6E, 0x65, 0x64, 0x62, 0x72, 0x74, 0x81, 0x71, 0x6F, 0x69, 0x63, 0x2E, 0x64, 0x2E, 0x73, 0x6D, 0x61, 0x72, 0x74, 0x44, 0x65, 0x76, 0x69, 0x63, 0x65, 0x62, 0x69, 0x66, 0x81, 0x68, 0x6F, 0x69, 0x63, 0x2E, 0x69, 0x66, 0x2E, 0x61 },
                        OicMessageContentType.ApplicationCbor)
                    .Returns(new List<IOicResource>
                    {
                        new CoreResources.OicResourceDirectory
                        {
                            ResourceTypes =  {"oic.wk.res"},
                            DeviceId = new Guid("0685B960-736F-46F7-BEC0-9E6CBD61ADC1"),
                            Links = new List<CoreResources.OicResourceLink>
                            {
                                new CoreResources.OicResourceLink
                                {
                                    Href = new Uri("/res", UriKind.Relative),
                                    Rel = "self",
                                    ResourceTypes =  {"oic.r.collection"},
                                    Interfaces = OicResourceInterface.LinkLists,
                                },
                                new CoreResources.OicResourceLink
                                {
                                    Href = new Uri("/smartDevice", UriKind.Relative),
                                    Rel = "contained",
                                    ResourceTypes =  {"oic.d.smartDevice"},
                                    Interfaces = OicResourceInterface.Actuator,
                                }
                            }
                        },
                        new CoreResources.OicResourceDirectory
                        {
                            ResourceTypes =  {"oic.wk.res"},
                            DeviceId = new Guid("0685B960-736F-46F7-BEC0-9E6CBD61ADC1"),
                            Links = new List<CoreResources.OicResourceLink>
                            {
                                new CoreResources.OicResourceLink
                                {
                                    Href = new Uri("/res", UriKind.Relative),
                                    Rel = "self",
                                    ResourceTypes =  {"oic.r.collection"},
                                    Interfaces = OicResourceInterface.LinkLists,
                                },
                                new CoreResources.OicResourceLink
                                {
                                    Href = new Uri("/smartDevice", UriKind.Relative),
                                    Rel = "contained",
                                    ResourceTypes =  {"oic.d.smartDevice"},
                                    Interfaces = OicResourceInterface.Actuator,
                                }
                            }
                        }
                    });
            }
        }
    }
}
