using Application.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace Infrastructure.Services
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _database;

        public RedisCacheService(IConnectionMultiplexer redis)
        {
            _redis = redis;
            _database = _redis.GetDatabase();
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            var serializedValue = JsonSerializer.Serialize(value);
            await _database.StringSetAsync(key, serializedValue, expiry);
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var value = await _database.StringGetAsync(key);
            if (value.IsNullOrEmpty)
            {
                return default;
            }
            return JsonSerializer.Deserialize<T>(value!);
        }

        public async Task RemoveAsync(string key)
        {
            await _database.KeyDeleteAsync(key);
        }

        // Example of how to use the RedisCacheService

        //public async Task SaveCustomerEventToCache(CustomerCreatedEvent customerEvent)
        //{
        //    var cacheKey = $"CustomerEvent:{customerEvent.CustomerId}";
        //    await _redisCacheService.SetAsync(cacheKey, customerEvent, TimeSpan.FromMinutes(10));
        //}

        //public async Task<CustomerCreatedEvent?> GetCustomerEventFromCache(Guid customerId)
        //{
        //    var cacheKey = $"CustomerEvent:{customerId}";
        //    return await _redisCacheService.GetAsync<CustomerCreatedEvent>(cacheKey);
        //}
    }
}
