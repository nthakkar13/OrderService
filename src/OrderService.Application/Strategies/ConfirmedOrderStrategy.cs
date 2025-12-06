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
    public class ConfirmedOrderStrategy : BaseOrderStrategy
    {
        public ConfirmedOrderStrategy(INotificationClient n, IMessageProducer k, ILogger<ConfirmedOrderStrategy> l) : base(n, k, l) { }

        public override OrderStatus TargetStatus => OrderStatus.Confirmed;
        protected override string AuditTag => "CONFIRMED_FLOW";
        protected override string KafkaTopicSuffix => "confirmed";
        // Aggressive 5 retries
        protected override AsyncPolicy RetryPolicy => Policy.Handle<HttpRequestException>()
            .WaitAndRetryAsync(5, _ => TimeSpan.FromSeconds(1));

        public new async Task ProcessOrderAsync(Order order)
        {
            // Specific logic: Fraud Detection
            if (order.TotalQuantity > 10)
            {
                _logger.LogWarning("Potential fraud detected: High-volume confirmed order. ID: {Id}", order.Id);
            }
            await base.ProcessOrderAsync(order);
        }
    }
}
