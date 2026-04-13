using Fixy.Domain.Entities;

namespace Fixy.Domain.Interfaces;

public interface INotificationRepository : IGenericRepository<Notification>
{
    Task<int> GetUnreadCountAsync(Guid userId);
    Task<Notification> MarkAsReadAsync(Guid notificationId, Guid userId);
    Task MarkAllAsReadAsync(Guid userId);
    Task DeleteAsync(Guid notificationId, Guid userId);
}
