using MediatR;
using OrderService.Application.Common;
using OrderService.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderService.Application.Orders.Queries
{
    public record GetOrderQuery(Guid Id) : IRequest<Order?>;

    public class GetOrderHandler : IRequestHandler<GetOrderQuery, Order?>
    {
        private readonly ICacheService _cache;

        public GetOrderHandler(ICacheService cache)
        {
            _cache = cache;
        }

        public async Task<Order?> Handle(GetOrderQuery request, CancellationToken cancellationToken)
        {
            string cacheKey = $"order:{request.Id}";

            // 1. Try Redis Cache
            var cachedOrder = await _cache.GetAsync<Order>(cacheKey);
            if (cachedOrder != null) return cachedOrder;

            // 2. Fallback to DB (Simulation)
            // In a real app: var dbOrder = await _dbContext.Orders.FindAsync(request.Id);
            var dbOrder = new Order
            {
                Id = request.Id,
                Status = OrderStatus.Confirmed,
                CustomerId = Guid.NewGuid()
            };

            // 3. Set Cache (5 minutes expiration)
            await _cache.SetAsync(cacheKey, dbOrder, TimeSpan.FromMinutes(5));

            return dbOrder;
        }
    }
}
