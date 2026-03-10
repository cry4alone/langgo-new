using Microsoft.Extensions.Caching.Distributed;

namespace LanggoNew.Shared.Infrastructure;

public interface IRedisCache
{
    Task<T?> GetDataAsync<T>(string key);
    Task SetDataAsync<T>(string key, T value, TimeSpan? expiration = null);
    Task RemoveDataAsync(string key);
}

public class RedisCache(IDistributedCache cache) : IRedisCache
{
    public async Task<T?> GetDataAsync<T>(string key)
    {

        var bytes = await cache.GetAsync(key);
        
        if (bytes == null || bytes.Length == 0)
            return default;
        
        var json = System.Text.Encoding.UTF8.GetString(bytes);
        return System.Text.Json.JsonSerializer.Deserialize<T>(json);
    }

    public async Task SetDataAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(value);
        var bytes = System.Text.Encoding.UTF8.GetBytes(json);
        
        var options = new DistributedCacheEntryOptions();
        if (expiration.HasValue)
            options.SetAbsoluteExpiration(expiration.Value);
        
        await cache.SetAsync(key, bytes, options);
    }

    public async Task RemoveDataAsync(string key)
    {
        await cache.RemoveAsync(key);
    }
}