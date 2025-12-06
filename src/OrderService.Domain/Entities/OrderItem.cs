using System;
using System.Collections.Generic;
using System.Text;

namespace OrderService.Domain
{
    public class OrderItem
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
