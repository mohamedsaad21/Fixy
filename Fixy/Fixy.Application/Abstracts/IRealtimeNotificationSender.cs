namespace Fixy.Application.Abstracts;

public interface IRealtimeNotificationSender
{
    Task SendToUserAsync(Guid userId, string method, object data);
    Task SendToGroupAsync(string groupName, string method, object data);
    Task SendToAllAsync(string method, object data);
    Task SendToUsersAsync(IEnumerable<Guid> userIds, string method, object data);
}
