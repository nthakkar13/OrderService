using System;
using System.Collections.Generic;
using System.Text;

namespace OrderService.Domain
{
    public class Order
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CustomerId { get; set; }
        public List<OrderItem> Items { get; set; } = new();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public OrderStatus Status { get; set; }

        // Helper to calculate total quantity
        public int TotalQuantity => Items.Sum(i => i.Quantity);
    }
}
