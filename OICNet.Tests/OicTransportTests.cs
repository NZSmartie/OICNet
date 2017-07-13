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
        private Mock<IOicInterface> _broadcaster;

        [TestInitialize]
        public void Setup()
        {
            _broadcaster = new Mock<IOicInterface>();
            _broadcaster
                .Setup(b => b.BroadcastMessageAsync(It.IsAny<OicMessage>()))
                .Returns(Task.CompletedTask)
                .Verifiable();
        }

        [TestMethod]
        public void BroadcastAllInterfaces()
        {
            var service = new OicClient();

            service.AddBroadcastInterface(_broadcaster.Object);
            service.Discover();

            Mock.VerifyAll(_broadcaster);
        }
    }
}
