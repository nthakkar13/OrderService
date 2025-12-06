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
    public class CanceledOrderStrategy : BaseOrderStrategy
    {
        public CanceledOrderStrategy(INotificationClient n, IMessageProducer k, ILogger<CanceledOrderStrategy> l) : base(n, k, l) { }

        public override OrderStatus TargetStatus => OrderStatus.Canceled;
        protected override string AuditTag => "CANCELLED_FLOW";
        protected override string KafkaTopicSuffix => ""; // Unused
        protected override AsyncPolicy RetryPolicy => Policy.NoOpAsync();

        // Override flags to disable behaviors
        protected override bool ShouldPublishToKafka => false;
        protected override bool ShouldNotify => false;
    }
}
