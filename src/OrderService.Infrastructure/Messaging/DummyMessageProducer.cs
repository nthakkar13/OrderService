using Microsoft.Extensions.Logging;
using OrderService.Application.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace OrderService.Infrastructure.Messaging
{
    public class DummyMessageProducer : IMessageProducer
    {
        private readonly ILogger<DummyMessageProducer> _logger;

        public DummyMessageProducer(ILogger<DummyMessageProducer> logger)
        {
            _logger = logger;
        }

        public Task PublishAsync(string topic, object message)
        {
            var jsonMessage = JsonSerializer.Serialize(message);

            _logger.LogWarning("DUMMY KAFKA: Published event to TOPIC: '{Topic}'. Payload: {Payload}",
                topic, jsonMessage);

            // In a real application, this is where Confluent.Kafka code would go.
            return Task.CompletedTask;
        }
    }
}
