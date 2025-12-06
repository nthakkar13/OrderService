using OrderService.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderService.Application.Common.Interfaces
{
    public interface INotificationClient
    {
        Task SendNotificationAsync(Order order);
    }
}
