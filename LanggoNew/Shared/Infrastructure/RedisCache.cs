using LanggoNew.Shared.Models;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace LanggoNew.Shared.Infrastructure;

public interface IRedisCache
{
    Task<T?> GetDataAsync<T>(string key);
    Task SetDataAsync<T>(string key, T value);
    Task RemoveDataAsync(string key);
    Task ExecuteWithLockAsync(string roomId, int userId,  Func<Task> action);
    IDatabase Database { get; } 
}

public class RedisCache : IRedisCache
{
    private readonly IDatabase _cache;
    public async Task ExecuteWithLockAsync(string roomId, int userId,  Func<Task> action)
    {
        var lockKey = $"lock:game:{roomId}";
        var lockValue = Guid.NewGuid().ToString();
        var accquired = await _cache.LockTakeAsync(lockKey, lockValue, TimeSpan.FromSeconds(5));
        
        if (!accquired)
            throw new Exception("Could not acquire lock");
        
        try
        {
            await action();
        }
        finally
        {
            _cache.LockRelease(lockKey, lockValue);
        }
    }

    public IDatabase Database => _cache;

    public RedisCache(IConnectionMultiplexer connectionMultiplexer)
    {
        _cache = connectionMultiplexer.GetDatabase();
    }

    public async Task<T?> GetDataAsync<T>(string key)
    {

        var value = await _cache.StringGetAsync(key);
        
        return value.IsNullOrEmpty ? default : System.Text.Json.JsonSerializer.Deserialize<T>(value);
    }

    public async Task SetDataAsync<T>(string key, T value)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(value);
        
        await _cache.StringSetAsync(key, json, TimeSpan.FromHours(1));
    }

    public async Task RemoveDataAsync(string key)
    {
        await _cache.KeyDeleteAsync(key);
    }
}