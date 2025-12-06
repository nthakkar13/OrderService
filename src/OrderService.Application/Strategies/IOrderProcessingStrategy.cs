using OrderService.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderService.Application.Strategies
{
    public interface IOrderProcessingStrategy
    {
        OrderStatus TargetStatus { get; }
        Task ProcessOrderAsync(Order order);
    }
}
