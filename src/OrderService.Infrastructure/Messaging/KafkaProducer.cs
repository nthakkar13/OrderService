using OrderService.Application.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderService.Infrastructure.Messaging
{
    public class KafkaProducer : IMessageProducer
    {
        // Configure Confluent.Kafka Producer here
        public Task PublishAsync(string topic, object message) => Task.CompletedTask;
    }
}
