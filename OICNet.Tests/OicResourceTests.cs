using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Moq.Language;
using NUnit.Framework;


namespace OICNet.Tests
{
    [TestFixture]
    public class OicResourceTests
    {
        private class TestResourceResolver : OicResolver
        {
            public TestResourceResolver()
            {
                _resourceTypes.Add("test.int", typeof(OicIntResouece));
                _resourceTypes.Add("test.number", typeof(OicNumberResouece));
            }
        }

        private Mock<IOicTransport> _mockTransport;
        private Mock<IOicEndpoint> _mockEndpoint;
        private OicConfiguration _configuration;

        [OneTimeSetUp]
        public void CreateOicConfiguration()
        {
            _configuration = new OicConfiguration {Resolver = new TestResourceResolver()};
            _configuration.Serialiser = new OicMessageSerialiser(_configuration.Resolver);
        }

        [SetUp]
        public void CreateTransportMocks()
        {
            _mockTransport = new Mock<IOicTransport>();
            _mockEndpoint = new Mock<IOicEndpoint>();
            _mockEndpoint.Setup(e => e.Transport).Returns(_mockTransport.Object);
        }

        [Test]
        public void TestRetreiveNullDevice()
        {
            Assert.Throws<NullReferenceException>(() =>
            {
                var repository = new OicRemoteResourceRepository();

                repository.RetrieveAsync(new OicCoreResource());
            });
        }

        [Test]
        public void TestRetreiveMultipleResults()
        {
            // Arrange
            _mockTransport
                    .Setup(t => t.SendMessageWithResponseAsync(It.IsAny<IOicEndpoint>(),
                        It.Is((OicRequest r) => r.Operation == OicRequestOperation.Get)))
                    .Returns(Task.FromResult(new OicResponse
                    {
                        ContentType = OicMessageContentType.ApplicationJson,
                        Content = Encoding.UTF8.GetBytes(
                            @"[{""if"":[""oic.if.baseline""],""rt"":[""oic.r.core""]},{""if"":[""oic.if.baseline""],""rt"":[""oic.r.core""]}]")
                    }));

            var resource = new OicCoreResource() { RelativeUri = "test" };
            var repository = new OicRemoteResourceRepository(new OicDevice(_mockEndpoint.Object));

            // Act
            TestDelegate operation = () => repository.RetrieveAsync(resource).Wait();

            // Assert
            Assert.Throws<InvalidOperationException>(operation);
        }

        [Test]
        [TestCaseSource(typeof(ResourceTestCaseSource), nameof(ResourceTestCaseSource.RetreiveTestCaseData))]
        public IOicResource TestRetreive(OicResponse response, IOicResource actual)
        {
            // Arrange
            _mockTransport
                .Setup(t => t.SendMessageWithResponseAsync(It.IsAny<IOicEndpoint>(),
                    It.Is((OicRequest r) => r.Operation == OicRequestOperation.Get)))
                .Returns(Task.FromResult(response));

            var repository = new OicRemoteResourceRepository(new OicDevice(_mockEndpoint.Object, _configuration));

            // Act
            repository.RetrieveAsync(actual).Wait();

            // Assert
            Mock.VerifyAll(_mockTransport);

            return actual;
        }
    }

    public static class ResourceTestCaseSource
    {
        public static IEnumerable RetreiveTestCaseData
        {
            get
            {
                yield return new TestCaseData(new OicResponse
                    {
                        ContentType = OicMessageContentType.ApplicationJson,
                        Content = Encoding.UTF8.GetBytes(@"{""if"":[""oic.if.baseline""],""rt"":[""oic.r.core""]}")
                    }, new OicCoreResource
                    {
                        RelativeUri = ""
                    })
                    .Returns(new OicCoreResource
                    {
                        RelativeUri = ""
                    });

                yield return new TestCaseData(new OicResponse
                    {
                        ContentType = OicMessageContentType.ApplicationJson,
                        Content = Encoding.UTF8.GetBytes(@"{""if"":[""oic.if.baseline""],""rt"":[""test.int""],""id"":""04d0e642-2b18-41fb-8983-7e60fba3be44"",""n"":""Integer"",value:1234}")
                    }, new OicIntResouece
                    {
                        RelativeUri = ""
                    })
                    .Returns(new OicIntResouece
                    {
                        Id = "04d0e642-2b18-41fb-8983-7e60fba3be44",
                        Name = "Integer",
                        Value = 1234,
                        RelativeUri = ""
                    });
    
                yield return new TestCaseData(new OicResponse
                    {
                        ContentType = OicMessageContentType.ApplicationJson,
                        Content = Encoding.UTF8.GetBytes(@"{""if"":[""oic.if.baseline""],""rt"":[""test.number""],""n"":""Number"",value:12.34,range:[0,100]}")
                    }, new OicNumberResouece
                    {
                        RelativeUri = ""
                    })
                    .Returns(new OicNumberResouece
                    {
                        Name = "Number",
                        Value = 12.34f,
                        Range = new List<float>{0,100},
                        RelativeUri = ""
                    });
            }
        }
    }
}
