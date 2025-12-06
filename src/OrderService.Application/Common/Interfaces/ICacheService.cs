using System;
using System.Collections.Generic;
using System.Text;

namespace OrderService.Application.Common
{
    public interface ICacheService
    {
        Task SetAsync<T>(string key, T value, TimeSpan expiration);
        Task<T?> GetAsync<T>(string key);
    }
}
