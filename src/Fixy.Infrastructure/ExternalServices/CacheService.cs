using Fixy.Application.Contracts.ExternalServices;
using StackExchange.Redis;
using System.Text.Json;

namespace Fixy.Infrastructure.ExternalServices;

public class CacheService : ICacheService
{
    private IDatabase _cacheDb;
    public CacheService(IConnectionMultiplexer redis)
    {
        _cacheDb = redis.GetDatabase();
    }
    public async Task<T> GetData<T>(string key)
    {
        var value = _cacheDb.StringGet(key);
        if (!string.IsNullOrEmpty(value))
            return JsonSerializer.Deserialize<T>((string)value);

        return default;
    }

    public async Task<bool> SetData<T>(string key, T value, TimeSpan expirationTime)
    {
        return _cacheDb.StringSet(key, JsonSerializer.Serialize(value), expirationTime);
    }

    public async Task<object> RemoveData(string key)
    {
        var _exist = _cacheDb.KeyExists(key);

        if (_exist)
            return _cacheDb.KeyDelete(key);

        return false;
    }
}
