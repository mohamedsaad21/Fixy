namespace Fixy.Application.Contracts.Services;

public interface IPresenceService
{
    Task SetOnlineAsync(Guid userId, string connectionId);
    Task SetOfflineAsync(string connectionId);
    Task<bool> IsOnlineAsync(Guid userId);
    Task JoinConversationAsync(Guid userId, Guid conversationId);
    Task LeaveConversationAsync(Guid userId);
    Task<bool> IsInConversationAsync(Guid userId, Guid conversationId);
}
