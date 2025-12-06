using Microsoft.Extensions.Logging;
using OrderService.Application.Common;
using OrderService.Application.Common.Interfaces;
using OrderService.Application.Strategies;
using OrderService.Domain;
using Polly;

public abstract class BaseOrderStrategy : IOrderProcessingStrategy
{
    protected readonly INotificationClient _notificationClient;
    protected readonly IMessageProducer _kafkaProducer;
    protected readonly ILogger _logger;

    protected BaseOrderStrategy(INotificationClient notificationClient, IMessageProducer kafkaProducer, ILogger logger)
    {
        _notificationClient = notificationClient;
        _kafkaProducer = kafkaProducer;
        _logger = logger;
    }

    public abstract OrderStatus TargetStatus { get; }
    protected abstract string AuditTag { get; }
    protected abstract string KafkaTopicSuffix { get; }
    protected abstract AsyncPolicy RetryPolicy { get; }
    protected virtual TimeSpan NotificationDelay => TimeSpan.Zero;
    protected virtual bool ShouldPublishToKafka => true;
    protected virtual bool ShouldNotify => true;

    public async Task ProcessOrderAsync(Order order)
    {
        _logger.LogInformation("Processing Order {OrderId} with Audit Tag: {Tag}", order.Id, AuditTag);

        // 1. Notification (with Delay and Retry)
        if (ShouldNotify)
        {
            if (NotificationDelay > TimeSpan.Zero)
            {
                _logger.LogInformation("Delaying notification for {Delay}...", NotificationDelay);
                await Task.Delay(NotificationDelay);
            }

            await RetryPolicy.ExecuteAsync(async () =>
            {
                _logger.LogInformation("Sending notification to NotificationService...");
                await _notificationClient.SendNotificationAsync(order);
            });
        }

        // 2. Kafka Publishing
        if (ShouldPublishToKafka)
        {
            var topic = $"orders.created.{KafkaTopicSuffix}";
            await _kafkaProducer.PublishAsync(topic, new { order.Id, order.Timestamp });
            _logger.LogInformation("Published to Kafka topic: {Topic}", topic);
        }
    }
}