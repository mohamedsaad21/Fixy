using Fixy.Application.Common.Helpers;
using Fixy.Application.Features.Notifications.Queries.GetNotifications;
using Fixy.Application.Resources;
using Fixy.Domain.Entities;
using Fixy.Domain.Helpers;
using Microsoft.Extensions.Localization;
using System.Text.Json;

namespace Fixy.Application.Mapping.Notifications.Queries;

public static class NotificationDomainToNotificationDtoMapping
{
    public static GetNotificationsResponse ToGetNotificationsResponse(this Notification notification, IStringLocalizer<SharedResources> localizer)
    {
        return new GetNotificationsResponse
        {
            Id = notification.Id,
            Type = EnumLocalizer.Localize(notification.Type, localizer),
            Title = localizer[notification.TitleKey],
            Body = localizer[notification.BodyKey],
            AdditionalData = JsonSerializer.Deserialize<Dictionary<string, string>>(notification.AdditionalDataJson),
            IsRead = notification.IsRead,
            CreatedAt = notification.CreatedAt.ToEgyptTime()
        };
    }
}
