using System;
using System.Collections;
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

            mockResolver.Setup(c => c.GetResourseType("oic.r.core")).Returns(typeof(OicCoreResource));
            mockResolver.Setup(c => c.GetResourseType("oic.r.audio")).Returns(typeof(ResourceTypes.Audio));

            _resolver = mockResolver.Object;
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
            return serialiser.Deserialise(input, type);
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
                    Interfaces = new List<OicResourceInterface> {OicResourceInterface.Baseline},
                    ResourceTypes = new List<string> {"oic.r.core"}
                }, OicMessageContentType.ApplicationJson).Returns(Encoding.UTF8.GetBytes(
                    "{\"rt\":[\"oic.r.core\"],\"if\":[\"oic.if.baseline\"],\"n\":\"Test\",\"id\":\"test\"}"));

                yield return new TestCaseData(new ResourceTypes.Audio()
                    {
                        Name = "AudioTest",
                        Id = "audio-test",
                        Interfaces = new List<OicResourceInterface> {OicResourceInterface.Baseline},
                        ResourceTypes = new List<string> {"oic.r.audio"},
                        Mute = false,
                        Volume = 75
                    }, OicMessageContentType.ApplicationJson)
                    .Returns(Encoding.UTF8.GetBytes(
                        "{\"rt\":[\"oic.r.audio\"],\"if\":[\"oic.if.baseline\"],\"n\":\"AudioTest\",\"id\":\"audio-test\",\"volume\":75,\"mute\":false}"));

                yield return new TestCaseData(new OicCoreResource
                {
                    Name = "Test",
                    Id = "test",
                    Interfaces = new List<OicResourceInterface> {OicResourceInterface.Baseline},
                    ResourceTypes = new List<string> {"oic.r.core"}
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
                        Interfaces = new List<OicResourceInterface> {OicResourceInterface.Baseline},
                        ResourceTypes = new List<string> {"oic.r.audio"},
                        Mute = false,
                        Volume = 75
                    }, OicMessageContentType.ApplicationCbor)
                    .Returns(new byte[]
                    {
                        0xA6, 0x62, 0x72, 0x74, 0x81, 0x6B, 0x6F, 0x69, 0x63, 0x2E, 0x72, 0x2E, 0x61, 0x75, 0x64, 0x69,
                        0x6F, 0x62, 0x69, 0x66, 0x81, 0x6F, 0x6F, 0x69, 0x63, 0x2E, 0x69, 0x66, 0x2E, 0x62, 0x61, 0x73,
                        0x65, 0x6C, 0x69, 0x6E, 0x65, 0x61, 0x6E, 0x69, 0x41, 0x75, 0x64, 0x69, 0x6F, 0x54, 0x65, 0x73,
                        0x74, 0x62, 0x69, 0x64, 0x6A, 0x61, 0x75, 0x64, 0x69, 0x6F, 0x2D, 0x74, 0x65, 0x73, 0x74, 0x66,
                        0x76, 0x6F, 0x6C, 0x75, 0x6D, 0x65, 0x18, 0x4B, 0x64, 0x6D, 0x75, 0x74, 0x65, 0xF4
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
                        Interfaces = new List<OicResourceInterface> {OicResourceInterface.Baseline},
                        ResourceTypes = new List<string> {"oic.r.core"}
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
                        Interfaces = new List<OicResourceInterface> {OicResourceInterface.Baseline},
                        ResourceTypes = new List<string> {"oic.r.core"}
                    });

                yield return new TestCaseData(
                        Encoding.UTF8.GetBytes(
                            "{\"rt\":[\"oic.r.audio\"],\"if\":[\"oic.if.baseline\"],\"n\":\"AudioTest\",\"id\":\"audio-test\",\"volume\":75,\"mute\":false}"),
                        OicMessageContentType.ApplicationJson)
                    .Returns(new ResourceTypes.Audio()
                    {
                        Name = "AudioTest",
                        Id = "audio-test",
                        Interfaces = new List<OicResourceInterface> {OicResourceInterface.Baseline},
                        ResourceTypes = new List<string> {"oic.r.audio"},
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
                        Interfaces = new List<OicResourceInterface> {OicResourceInterface.Baseline},
                        ResourceTypes = new List<string> {"oic.r.audio"},
                        Mute = false,
                        Volume = 75
                    });
            }
        }
    }
}
