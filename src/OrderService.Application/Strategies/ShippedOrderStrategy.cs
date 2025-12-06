using Microsoft.Extensions.Logging;
using OrderService.Application.Common;
using OrderService.Application.Common.Interfaces;
using OrderService.Domain;
using Polly;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderService.Application.Strategies
{
    public class ShippedOrderStrategy : BaseOrderStrategy
    {
        public ShippedOrderStrategy(INotificationClient n, IMessageProducer k, ILogger<ShippedOrderStrategy> l) : base(n, k, l) { }

        public override OrderStatus TargetStatus => OrderStatus.Shipped;
        protected override string AuditTag => "SHIPPED_FLOW";
        protected override string KafkaTopicSuffix => "shipped";
        protected override TimeSpan NotificationDelay => TimeSpan.FromSeconds(5);
        // Passive 2 retries
        protected override AsyncPolicy RetryPolicy => Policy.Handle<HttpRequestException>().WaitAndRetryAsync(2, _ => TimeSpan.FromSeconds(5));
    }
}
