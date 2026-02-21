using Fixy.Application.Features.Notifications.Queries.GetNotifications;
using Fixy.Domain.Entities;

namespace Fixy.Application.Mapping.Notifications.Queries;

public static class NotificationDomainToNotificationDtoMapping
{
    public static GetNotificationsDto ToNotificationsDto(this Notification notification)
    {
        return new GetNotificationsDto
        {
            Id = notification.Id,
            Type = notification.Type.ToString(),
            Data = notification.Data,
            IsRead = notification.IsRead,
            CreatedAt = notification.CreatedAt
        };
    }
}
