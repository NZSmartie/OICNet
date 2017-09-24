using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Moq.Language;
using NUnit.Framework;
using System.Threading;
using OICNet.Tests.Mocks;

namespace OICNet.Tests
{
    [TestFixture]
    public class OicResourceDiscoverClientTests
    {
        private Mock<MockOicTransport> _mockTransport;

        [SetUp]
        public void SetupMockTransport()
        {
            _mockTransport = new Mock<MockOicTransport>() { CallBase = true };
            _mockTransport
                .Setup(i => i.BroadcastMessageAsync(It.IsAny<OicMessage>()))
                .Callback<OicMessage>(request => _mockTransport.Object.EnqueueReceivePacket(
                    new OicReceivedMessage
                    {
                        Endpoint = null,
                        Message = new OicResourceResponse(OicConfiguration.Default, new OicResourceList
                        {
                            new CoreResources.OicResourceDirectory
                            {
                                DeviceId = Guid.Parse("0685B960-736F-46F7-BEC0-9E6CBD61ADC1"),
                                Links =
                                {
                                    new CoreResources.OicResourceLink
                                    {
                                        Href = new Uri("/oid/d", UriKind.Relative),
                                        ResourceTypes = {"oic.d.light", "oic.wk.d"},
                                        Interfaces = OicResourceInterface.ReadOnly | OicResourceInterface.Baseline,
                                    },
                                    new CoreResources.OicResourceLink
                                    {
                                        Href = new Uri("/oid/p", UriKind.Relative),
                                        ResourceTypes = {"oic.wk.p"},
                                        Interfaces = OicResourceInterface.ReadOnly | OicResourceInterface.Baseline,
                                    },
                                    new CoreResources.OicResourceLink
                                    {
                                        Href = new Uri("/switch", UriKind.Relative),
                                        ResourceTypes = {"oic.r.switch.binary"},
                                        Interfaces = OicResourceInterface.Actuator | OicResourceInterface.Baseline,
                                    },
                                    new CoreResources.OicResourceLink
                                    {
                                        Href = new Uri("/brightness", UriKind.Relative),
                                        ResourceTypes = {"oic.r.light.brightness"},
                                        Interfaces = OicResourceInterface.Actuator | OicResourceInterface.Baseline,
                                    }
                                }
                           }
                        })
                        { RequestId = request.RequestId, ResposeCode = OicResponseCode.Content }
                    }))
                .CallBase();
        }

        [Test]
        public async Task TestDiscoverOnAllInterfaces()
        {
            // Arrange
            var mockInterface1 = new Mock<IOicTransport>();
            mockInterface1
                .Setup(b => b.BroadcastMessageAsync(It.IsAny<OicRequest>()))
                .Returns(Task.CompletedTask);
            var mockInterface2 = new Mock<IOicTransport>();
            mockInterface2
                .Setup(b => b.BroadcastMessageAsync(It.IsAny<OicRequest>()))
                .Returns(Task.CompletedTask);

            var client = new OicClient();

            client.AddTransport(mockInterface1.Object);
            client.AddTransport(mockInterface2.Object);

            // Act
            await new OicResourceDiscoverClient(client).DiscoverAsync();

            // Assert
            Mock.VerifyAll(mockInterface1, mockInterface2);
        }

        [Test]
        public void TestAddNullTransport()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var client = new OicClient();

                client.AddTransport(null);
            });
        }

        [Test]
        public async Task TestDiscoverDevice()
        {
            //Arange 
            bool newDeviceCallbackInvoked = false;
            OicDevice actualDevice = null;

            // Act
            using (var client = new OicClient())
            {
                var service = new OicResourceDiscoverClient(client);

                client.AddTransport(_mockTransport.Object);

                service.NewDevice += (s, e) =>
                {
                    newDeviceCallbackInvoked = true;
                    actualDevice = e.Device;
                };

                await service.DiscoverAsync();

                await Task.Delay(100);
            }

            // Assert
            Assert.IsTrue(newDeviceCallbackInvoked, $"{typeof(OicResourceDiscoverClient)}.{nameof(OicResourceDiscoverClient.NewDevice)} was not invoked");

            var expectedDevice = new OicDevice()
            {
                DeviceId = Guid.Parse("0685B960-736F-46F7-BEC0-9E6CBD61ADC1"),
                Resources = {
                    new CoreResources.OicDeviceResource
                    {
                        RelativeUri = "/oid/d",
                        ResourceTypes = {"oic.d.light", "oic.wk.d"},
                        Interfaces = OicResourceInterface.ReadOnly | OicResourceInterface.Baseline,
                    },
                    new CoreResources.OicPlatformResource
                    {
                        RelativeUri = "/oid/p",
                        ResourceTypes = {"oic.wk.p"},
                        Interfaces = OicResourceInterface.ReadOnly | OicResourceInterface.Baseline,
                    },
                    new ResourceTypes.SwitchBinary
                    {
                        RelativeUri = "/switch",
                        ResourceTypes = {"oic.r.switch.binary"},
                        Interfaces = OicResourceInterface.Actuator | OicResourceInterface.Baseline,
                    },
                    new ResourceTypes.LightBrightness
                    {
                        RelativeUri = "/brightness",
                        ResourceTypes = {"oic.r.light.brightness"},
                        Interfaces = OicResourceInterface.Actuator | OicResourceInterface.Baseline,
                    }
                }
            };
            Assert.AreEqual(expectedDevice.DeviceId, actualDevice.DeviceId);
            Assert.AreEqual(expectedDevice.Resources, actualDevice.Resources);
        }

        [Test]
        public async Task Discover_InvokedTwice_TriggersNewDeviceOnce()
        {
            //Arange 
            int newDeviceCallbackInvokations = 0;

            // Act
            using (var client = new OicClient())
            {
                var service = new OicResourceDiscoverClient(client);

                client.AddTransport(_mockTransport.Object);

                service.NewDevice += (s, e) =>
                {
                    newDeviceCallbackInvokations++;
                };

                await service.DiscoverAsync();

                await Task.Delay(100);

                await service.DiscoverAsync();

                await Task.Delay(100);
            }

            // Assert
            Assert.That(newDeviceCallbackInvokations, Is.EqualTo(1), $"{typeof(OicResourceDiscoverClient)}.{nameof(OicResourceDiscoverClient.NewDevice)} was not invoked once");
        }

        [Test]
        public async Task Discover_InvokedTwiceAndCleared_TriggersNewDeviceTwice()
        {
            //Arange 
            int newDeviceCallbackInvokations = 0;

            // Act
            using (var client = new OicClient())
            {
                var service = new OicResourceDiscoverClient(client);

                client.AddTransport(_mockTransport.Object);

                service.NewDevice += (s, e) =>
                {
                    newDeviceCallbackInvokations++;
                };

                await service.DiscoverAsync();

                await Task.Delay(100);

                await service.DiscoverAsync(true);

                await Task.Delay(100);
            }

            // Assert
            Assert.That(newDeviceCallbackInvokations, Is.EqualTo(2), $"{typeof(OicResourceDiscoverClient)}.{nameof(OicResourceDiscoverClient.NewDevice)} was not invoked twice");
        }
    }
}
