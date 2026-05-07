using Fixy.Domain.Entities;
using Fixy.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Infrastructure.Persistence.Repositories;

public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
{
    private readonly DbSet<Notification> _notifications;

    public NotificationRepository(FixyDbContext dbContext) : base(dbContext)
    {
        _notifications = dbContext.Set<Notification>();
    }

    public async Task<int> GetUnreadCountAsync(Guid userId)
    {
        return await _notifications.Where(n => n.UserId == userId && !n.IsRead)
            .CountAsync();
    }

    public async Task<Notification> MarkAsReadAsync(Guid notificationId, Guid userId)
    {
        var notification = await _notifications.FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

        if (notification != null && !notification.IsRead)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTimeOffset.UtcNow;
        }
        return notification;
    }

    public async Task MarkAllAsReadAsync(Guid userId)
    {
        var unreadNotifications = await _notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync();

        foreach (var notification in unreadNotifications)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTimeOffset.UtcNow;
        }
    }

    public async Task DeleteAsync(Guid notificationId, Guid userId)
    {
        var notification = await _notifications.FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

        if (notification != null)
        {
            _notifications.Remove(notification);
        }
    }
}
