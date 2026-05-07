using FirebaseAdmin.Messaging;
using Fixy.Application.Common.Helpers;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Resources;
using Fixy.Domain.Entities.Identity;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using Fixy.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Localization;

namespace Fixy.Infrastructure.Services;

public class NotificationService(IHubContext<NotificationHub> hubContext, IUnitOfWork unitOfWork, 
    IStringLocalizer<SharedResources> localizer) : INotificationService
{
    public async Task SendFullNotificationAsync(ApplicationUser user, NotificationType type, string titleKey, string bodyKey)
    {
        await SaveNotificationAsync(user.Id, type, titleKey, bodyKey);

        CultureHelper.SetCulture(user.PreferredLanguage);

        var title = localizer[titleKey];
        var message = localizer[bodyKey];

        var payload = new
        {
            title,
            message,
            type = EnumLocalizer.Localize(type, localizer),
            createdAt = DateTimeOffset.UtcNow
        };

        await SendNotificationToUserAsync(user, payload);

        if (!string.IsNullOrEmpty(user.FcmToken))
        {
            await SendPushNotificationAsync(
                fcmToken: user.FcmToken,
                title: title,
                body: message,
                data: new Dictionary<string, string>
                {
                    { "type", EnumLocalizer.Localize(type, localizer) },
                    { "createdAt", DateTimeOffset.UtcNow.ToString("O") }
                }
            );
        }
    }

    private async Task SaveNotificationAsync(Guid userId, NotificationType type, string titleKey, string bodyKey)
    {
        var notification = new Domain.Entities.Notification
        {
            UserId = userId,
            Type = type,
            TitleKey = titleKey,
            BodyKey = bodyKey,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        await unitOfWork.Notifications.AddAsync(notification);
    }
    private async Task SendNotificationToUserAsync(ApplicationUser user, object payload, CancellationToken cancellationToken = default)
    {
        await hubContext.Clients.Group($"user_{user.Id}").SendAsync("ReceiveNotification", payload, cancellationToken);
    }

    private async Task SendPushNotificationAsync(string fcmToken, string title, string body, Dictionary<string, string>? data = null, CancellationToken cancellationToken = default)
    {
        var message = new Message
        {
            Token = fcmToken,
            Notification = new Notification
            {
                Title = title,
                Body = body
            },
            Data = data ?? new Dictionary<string, string>(),
            Android = new AndroidConfig
            {
                Priority = Priority.High
            },
            Apns = new ApnsConfig
            {
                Aps = new Aps { Sound = "default" }
            }
        };

        await FirebaseMessaging.DefaultInstance.SendAsync(message, cancellationToken);
    }
}