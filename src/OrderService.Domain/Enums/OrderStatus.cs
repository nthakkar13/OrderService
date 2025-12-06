using System;
using System.Collections.Generic;
using System.Text;

namespace OrderService.Domain
{
    public enum OrderStatus
    {
        Pending,
        Confirmed,
        Shipped,
        Canceled
    }
}
