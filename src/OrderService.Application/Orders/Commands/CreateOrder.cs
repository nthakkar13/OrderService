using MediatR;
using OrderService.Application.Common;
using OrderService.Application.Strategies;
using OrderService.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderService.Application.Orders.Commands
{
    public record CreateOrderCommand(Guid CustomerId, List<OrderItem> Items, OrderStatus Status) : IRequest<Guid>;
    public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, Guid>
    {
        private readonly IEnumerable<IOrderProcessingStrategy> _strategies;
        private readonly ICacheService _cache; // Simulating dependency for DB/Repository

        public CreateOrderHandler(IEnumerable<IOrderProcessingStrategy> strategies, ICacheService cache)
        {
            _strategies = strategies;
            _cache = cache;
        }

        public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = new Order
            {
                CustomerId = request.CustomerId,
                Items = request.Items,
                Status = request.Status
            };

            // 1. Simulate Saving to DB (omitted)

            // 2. Select Strategy based on Status
            var strategy = _strategies.FirstOrDefault(s => s.TargetStatus == order.Status);

            if (strategy == null)
                throw new InvalidOperationException($"No processing strategy found for status: {order.Status}");

            // 3. Execute Strategy (Notification, Kafka, Logging, Delays, Retries)
            await strategy.ProcessOrderAsync(order);

            // 4. Cache the result immediately
            await _cache.SetAsync($"order:{order.Id}", order, TimeSpan.FromMinutes(5));

            return order.Id;
        }

    }
}
