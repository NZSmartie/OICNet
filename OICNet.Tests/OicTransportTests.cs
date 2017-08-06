using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Moq.Language;
using NUnit.Framework;


namespace OICNet.Tests
{
    [TestFixture]
    public class OicTransportTests
    {
        private Mock<IOicInterface> _broadcaster;

        [SetUp]
        public void Setup()
        {
            _broadcaster = new Mock<IOicInterface>();
            _broadcaster
                .Setup(b => b.BroadcastMessageAsync(It.IsAny<OicRequest>()))
                .Returns(Task.CompletedTask)
                .Verifiable();
        }

        [Test]
        public void BroadcastAllInterfaces()
        {
            var service = new OicDiscoverService();

            service.AddInterface(_broadcaster.Object);
            service.Discover();

            Mock.VerifyAll(_broadcaster);
        }
    }
}
