﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Moq.Language;
using NUnit.Framework;
using OICNet.Utilities;

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
            _configuration = new OicConfiguration(new TestResourceResolver());
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
            Assert.Throws<ArgumentNullException>(() =>
            {
                // TODO: Test only OicremoteDevice null
                new OicRemoteResourceRepository(null, null);

                // TODO: Test only OicClient null
                new OicRemoteResourceRepository(null, null);
            });
        }

        //[Test]
        //public void TestRetreiveMultipleResults()
        //{
        //    // Arrange
        //    _mockTransport
        //            .Setup(t => t.SendMessageWithResponseAsync(It.IsAny<IOicEndpoint>(),
        //                It.Is((OicRequest r) => r.Operation == OicRequestOperation.Get)))
        //            .Returns(Task.FromResult(new OicResponse
        //            {
        //                ContentType = OicMessageContentType.ApplicationJson,
        //                Content = Encoding.UTF8.GetBytes(
        //                    @"[{""if"":[""oic.if.baseline""],""rt"":[""oic.r.core""]},{""if"":[""oic.if.baseline""],""rt"":[""oic.r.core""]}]")
        //            }));

        //    var repository = new OicRemoteResourceRepository(new OicRemoteDevice(_mockEndpoint.Object));

        //    // Act
        //    AsyncTestDelegate operation = async () => await repository.RetrieveAsync("test");

        //    // Assert
        //    Assert.ThrowsAsync<InvalidOperationException>(operation);
        //}

        [Test]
        [TestCaseSource(typeof(ResourceTestCaseSource), nameof(ResourceTestCaseSource.RetreiveTestCaseData))]
        public async Task<IOicResource> TestRetreive(OicResponse response, IOicResource actual)
        {
            throw new NotImplementedException("Api changed");
            // Arrange
            //_mockTransport
            //    .Setup(t => t.SendMessageWithResponseAsync(It.IsAny<IOicEndpoint>(),
            //        It.Is((OicRequest r) => r.Operation == OicRequestOperation.Get)))
            //    .Returns(Task.FromResult(response));

            //var repository = new OicRemoteResourceRepository(new OicRemoteDevice(_mockEndpoint.Object));

            //// Act
            //var result = await repository.RetrieveAsync(OicRequest.Create(actual.RelativeUri));

            //actual = result.GetResource(_configuration);

            //// Assert
            //Mock.VerifyAll(_mockTransport);

            //return actual;
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
