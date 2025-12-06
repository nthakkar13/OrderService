using System;
using System.Collections.Generic;
using System.Text;

namespace OrderService.Application.Common
{
    public interface IMessageProducer
    {
        Task PublishAsync(string topic, object message);
    }
}
