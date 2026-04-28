using FirebaseAdmin.Messaging;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Interfaces;
using Fixy.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace Fixy.Infrastructure.Services;

public class NotificationService(IHubContext<NotificationHub> hubContext, IUnitOfWork unitOfWork) : INotificationService
{
    public async Task SaveNotificationAsync(Guid userId, string type, object data)
    {
        var notification = new Domain.Entities.Notification
        {
            UserId = userId,
            Type = type,
            Data = JsonSerializer.Serialize(data),
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        await unitOfWork.Notifications.AddAsync(notification);
    }
    public async Task SendNotificationToUserAsync(Guid userId, object payload, CancellationToken cancellationToken = default)
    {
        await hubContext.Clients.Group($"user_{userId}").SendAsync("ReceiveNotification", payload, cancellationToken);
    }

    public async Task SendPushNotificationAsync(string fcmToken, string title, string body, Dictionary<string, string>? data = null, CancellationToken cancellationToken = default)
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