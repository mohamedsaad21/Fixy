using FirebaseAdmin.Messaging;
using Fixy.Application.Common.DTOs.Notifications;
using Fixy.Application.Common.Helpers;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Resources;
using Fixy.Domain.Entities.Identity;
using Fixy.Domain.Enums;
using Fixy.Domain.Helpers;
using Fixy.Domain.Interfaces;
using Fixy.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Fixy.Infrastructure.Services;

public class NotificationService(IHubContext<NotificationHub> hubContext, IUnitOfWork unitOfWork,
    IStringLocalizer<SharedResources> localizer, ILogger<NotificationService> logger) : INotificationService
{
    public async Task SendFullNotificationAsync(Guid userId, NotificationType type, string titleKey, string bodyKey, Dictionary<string, string>? additionalData = null)
    {
        var user = await unitOfWork.Users.GetTableNoTracking().FirstOrDefaultAsync(x => x.Id == userId);
        if (user is null)
        {
            logger.LogWarning("User {UserId} not found for notification", userId);
            return;
        }

        CultureHelper.SetCulture(user.PreferredLanguage);

        await SaveNotificationAsync(user.Id, type, titleKey, bodyKey, additionalData);
        await unitOfWork.SaveChangesAsync();

        var title = localizer[titleKey].Value;
        var message = localizer[bodyKey].Value;

        var payload = new NotificationPayload(title, message, EnumLocalizer.Localize(type, localizer), DateTimeOffset.UtcNow.ToEgyptTime(), additionalData);

        try
        {
            await SendNotificationToUserAsync(user, payload);
            if (!string.IsNullOrEmpty(user.FcmToken))
            {
                var fcmData = new Dictionary<string, string>
                {
                    { "type", EnumLocalizer.Localize(type, localizer) },
                    { "createdAt", DateTimeOffset.UtcNow.ToString("O") }
                };

                if (additionalData != null)
                {
                    foreach (var kvp in additionalData)
                    {
                        fcmData[kvp.Key] = kvp.Value;
                    }
                }

                await SendPushNotificationAsync(
                    fcmToken: user.FcmToken,
                    title: title,
                    body: message,
                    data: fcmData
                );
            }

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to deliver real-time notification to UserId: {UserId}", user.Id);
        }
        finally
        {
            CultureHelper.SetCulture("en");
        }
    }

    private async Task SaveNotificationAsync(Guid userId, NotificationType type, string titleKey, string bodyKey, Dictionary<string, string>? additionalData = null)
    {
        var notification = new Domain.Entities.Notification
        {
            UserId = userId,
            Type = type,
            TitleKey = titleKey,
            BodyKey = bodyKey,
            AdditionalDataJson = additionalData is not null? JsonSerializer.Serialize(additionalData)
            : null,
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