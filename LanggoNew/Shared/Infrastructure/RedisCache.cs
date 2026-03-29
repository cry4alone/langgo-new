using StackExchange.Redis;

namespace LanggoNew.Shared.Infrastructure;

public interface IRedisCache
{
    Task<T?> GetDataAsync<T>(string key);
    Task SetDataAsync<T>(string key, T value);
    Task RemoveDataAsync(string key);
    Task ExecuteWithLockAsync(string roomId, Func<string, Task> action);
    Task<T> ExecuteWithLockAsync<T>(string roomId, Func<string, Task<T>> action);
    IDatabase Database { get; }
}

public class RedisCache(IConnectionMultiplexer connectionMultiplexer) : IRedisCache
{
    private readonly IDatabase _cache = connectionMultiplexer.GetDatabase();

    public IDatabase Database => _cache;

    public async Task ExecuteWithLockAsync(string roomId, Func<string, Task> action)
    {
        await ExecuteWithLockAsync<object?>(roomId, async gameKey =>
        {
            await action(gameKey);
            return null;
        });
    }

    public async Task<T> ExecuteWithLockAsync<T>(string roomId, Func<string, Task<T>> action)
    {
        var gameKey = $"game:{roomId}";
        var lockKey = $"lock:{gameKey}";
        var lockValue = Guid.NewGuid().ToString();

        var acquired = await _cache.LockTakeAsync(lockKey, lockValue, TimeSpan.FromSeconds(5));
        if (!acquired)
            throw new Exception("Could not acquire lock");

        try
        {
            return await action(gameKey);
        }
        finally
        {
            await _cache.LockReleaseAsync(lockKey, lockValue);
        }
    }

    public async Task<T?> GetDataAsync<T>(string key)
    {
        var value = await _cache.StringGetAsync(key);
        return value.IsNullOrEmpty ? default : System.Text.Json.JsonSerializer.Deserialize<T>(value!);
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