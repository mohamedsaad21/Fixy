using Fixy.Application.Contracts.Services;
using StackExchange.Redis;

namespace Fixy.Infrastructure.Services;

public class PresenceService : IPresenceService
{
    private readonly IConnectionMultiplexer _redis;

    public PresenceService(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    public async Task SetOnlineAsync(Guid userId, string connectionId)
    {
        var db = _redis.GetDatabase();

        // Add connectionId to the user's connection set
        await db.SetAddAsync($"online:{userId}", connectionId);
        await db.KeyExpireAsync($"online:{userId}", TimeSpan.FromHours(24));

        // Store reverse mapping: connectionId → userId
        // So we can look up userId in SetOfflineAsync using only connectionId
        await db.StringSetAsync($"conn:{connectionId}", userId.ToString(), TimeSpan.FromHours(24)
        );
    }

    public async Task SetOfflineAsync(string connectionId)
    {
        var db = _redis.GetDatabase();

        // Look up which user owns this connectionId
        var userIdStr = await db.StringGetAsync($"conn:{connectionId}");
        if (userIdStr.IsNullOrEmpty) return;

        var userId = Guid.Parse(userIdStr.ToString()!);

        // Remove this specific connection from the user's set
        await db.SetRemoveAsync($"online:{userId}", connectionId);

        // Delete the reverse mapping — no longer needed
        await db.KeyDeleteAsync($"conn:{connectionId}");
    }

    public async Task<bool> IsOnlineAsync(Guid userId)
    {
        var db = _redis.GetDatabase();

        // User is online only if they have at least one active connection
        // KeyExistsAsync is wrong here — an empty set key can still exist
        var connectionCount = await db.SetLengthAsync($"online:{userId}");
        return connectionCount > 0;
    }

    public async Task JoinConversationAsync(Guid userId, Guid conversationId)
    {
        var db = _redis.GetDatabase();
        await db.StringSetAsync($"inconvo:{userId}", conversationId.ToString(), TimeSpan.FromHours(24)
        );
    }

    public async Task LeaveConversationAsync(Guid userId)
    {
        var db = _redis.GetDatabase();
        await db.KeyDeleteAsync($"inconvo:{userId}");
    }

    public async Task<bool> IsInConversationAsync(Guid userId, Guid conversationId)
    {
        var db = _redis.GetDatabase();
        var current = await db.StringGetAsync($"inconvo:{userId}");
        return current == conversationId.ToString();
    }
}