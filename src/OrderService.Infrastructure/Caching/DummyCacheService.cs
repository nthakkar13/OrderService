using Microsoft.Extensions.Logging;
using OrderService.Application.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace OrderService.Infrastructure.Caching
{
    public class DummyCacheService : ICacheService
    {
        private readonly ILogger<DummyCacheService> _logger;
        // Use ConcurrentDictionary to simulate cache storage in memory
        private static readonly ConcurrentDictionary<string, (string Value, DateTimeOffset Expiry)> _cacheStore
            = new();

        public DummyCacheService(ILogger<DummyCacheService> logger)
        {
            _logger = logger;
        }

        public Task SetAsync<T>(string key, T value, TimeSpan expiration)
        {
            var jsonValue = JsonSerializer.Serialize(value);
            var expiryTime = DateTimeOffset.UtcNow.Add(expiration);

            _cacheStore[key] = (jsonValue, expiryTime);
            _logger.LogInformation("DUMMY CACHE: Set key '{Key}' with expiration {Expiration}", key, expiration);

            return Task.CompletedTask;
        }

        public Task<T?> GetAsync<T>(string key)
        {
            if (_cacheStore.TryGetValue(key, out var entry))
            {
                if (entry.Expiry > DateTimeOffset.UtcNow)
                {
                    // Cache hit
                    _logger.LogInformation("DUMMY CACHE: Hit for key '{Key}'.", key);
                    var result = JsonSerializer.Deserialize<T>(entry.Value);
                    return Task.FromResult(result);
                }
                else
                {
                    // Cache expired, remove it
                    _cacheStore.TryRemove(key, out _);
                    _logger.LogWarning("DUMMY CACHE: Miss for key '{Key}' (Expired).", key);
                }
            }

            // Cache miss
            _logger.LogWarning("DUMMY CACHE: Miss for key '{Key}'.", key);
            return Task.FromResult(default(T));
        }
    }
}
