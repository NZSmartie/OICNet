using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;
using Moq.Language;

namespace OICNet.Tests
{
    [TestClass]
    public class OicTransportTests
    {
        private Mock<IOicBroadcaster> _broadcaster;
        private Mock<IOicTransportProvider> _transportProvider;

        [TestInitialize]
        public void Setup()
        {
            _broadcaster = new Mock<IOicBroadcaster>();
            _broadcaster
                .Setup(b => b.SendBroadcastAsync(It.IsAny<OicMessage>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _transportProvider = new Mock<IOicTransportProvider>();
            _transportProvider.Setup(t => t.GetBroadcasters()).Returns(new List<IOicBroadcaster> { _broadcaster.Object });
        }

        [TestMethod]
        public void BroadcastAllInterfaces()
        {
            var service = new OicService();

            service.AddTransportProvider(_transportProvider.Object);
            service.Discover();

            Mock.VerifyAll(_transportProvider, _broadcaster);
        }
    }
}
