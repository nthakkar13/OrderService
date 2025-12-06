using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using OrderService.Application.Common;
using StackExchange.Redis;
namespace OrderService.Infrastructure.Caching
{
    public class RedisCacheService : ICacheService
    {
        private readonly IConnectionMultiplexer _redis;
        public RedisCacheService(IConnectionMultiplexer redis) => _redis = redis;

        public async Task<T?> GetAsync<T>(string key)
        {
            var db = _redis.GetDatabase();
            var val = await db.StringGetAsync(key);
            if (val.IsNullOrEmpty)
            {
                return default;
            }
            string jsonString = val.ToString();     
            return JsonSerializer.Deserialize<T>(jsonString);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan expiration)
        {
            var db = _redis.GetDatabase();
            await db.StringSetAsync(key, JsonSerializer.Serialize(value), expiration);
        }
    }
}
