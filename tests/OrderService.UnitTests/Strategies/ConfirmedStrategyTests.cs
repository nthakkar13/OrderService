using Microsoft.Extensions.Logging;
using Moq;
using OrderService.Application.Common;
using OrderService.Application.Common.Interfaces;
using OrderService.Application.Strategies;
using OrderService.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderService.UnitTests.Strategies
{
    public class ConfirmedStrategyTests
    {
        private readonly Mock<INotificationClient> _mockNotifier;
        private readonly Mock<IMessageProducer> _mockProducer;
        private readonly Mock<ILogger<ConfirmedOrderStrategy>> _mockLogger;
        private readonly ConfirmedOrderStrategy _strategy;

        public ConfirmedStrategyTests()
        {
            // Setup Mocks
            _mockNotifier = new Mock<INotificationClient>();
            _mockProducer = new Mock<IMessageProducer>();

            // Setup Logger Mock using a verifiable method signature
            _mockLogger = new Mock<ILogger<ConfirmedOrderStrategy>>();

            // Instantiate the Strategy using the Mocks
            _strategy = new ConfirmedOrderStrategy(
                _mockNotifier.Object,
                _mockProducer.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task ProcessOrderAsync_HighVolume_LogsFraudWarning()
        {
            // ARRANGE
            var highVolumeOrder = new Order
            {
                Id = Guid.NewGuid(),
                Status = OrderStatus.Confirmed,
                Items = new List<OrderItem> { new() { Quantity = 11 } } // Total quantity > 10
            };

            // ACT
            await _strategy.ProcessOrderAsync(highVolumeOrder);

            // ASSERT

            // 1. Verify Fraud Check: Check if the logger was called with the specific warning message
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning, // We expect a warning log
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Potential fraud detected")), // Check message content
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
                Times.Once, // Must be called exactly once
                "The fraud warning must be logged for high-volume orders."
            );

            // 2. Verify Delegation (Base Logic): Check if base class dependencies were called
            _mockNotifier.Verify(n => n.SendNotificationAsync(It.IsAny<Order>()), Times.Once,
                "Notification must still be sent.");
            _mockProducer.Verify(p => p.PublishAsync(It.IsAny<string>(), It.IsAny<object>()), Times.Once,
                "Kafka event must still be published.");
        }

        [Fact]
        public async Task ProcessOrderAsync_LowVolume_DoesNotLogFraudWarning()
        {
            // ARRANGE
            var lowVolumeOrder = new Order
            {
                Id = Guid.NewGuid(),
                Status = OrderStatus.Confirmed,
                Items = new List<OrderItem> { new() { Quantity = 5 } } // Total quantity <= 10
            };

            // ACT
            await _strategy.ProcessOrderAsync(lowVolumeOrder);

            // ASSERT

            // 1. Verify Fraud Check: Ensure the fraud log was NOT called
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning, // Check for a warning log
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Potential fraud detected")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
                Times.Never, // Must NOT be called
                "The fraud warning must NOT be logged for low-volume orders."
            );

            // 2. Verify Delegation (Base Logic): Base logic must still execute
            _mockNotifier.Verify(n => n.SendNotificationAsync(It.IsAny<Order>()), Times.Once,
                "Notification must still be sent.");
        }
    }
}
