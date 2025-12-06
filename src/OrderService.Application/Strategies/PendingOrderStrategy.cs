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
    public class PendingOrderStrategy : BaseOrderStrategy
    {
        public PendingOrderStrategy(INotificationClient n, IMessageProducer k, ILogger<PendingOrderStrategy> l) : base(n, k, l) { }

        public override OrderStatus TargetStatus => OrderStatus.Pending;
        protected override string AuditTag => "PENDING_FLOW";
        protected override string KafkaTopicSuffix => "pending";
        // Default 3 retries
        protected override AsyncPolicy RetryPolicy => Policy.Handle<HttpRequestException>()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }
}
