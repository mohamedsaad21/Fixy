using FirebaseAdmin.Messaging;
using Fixy.Application.Contracts.Services;
using Fixy.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Fixy.Infrastructure.Services;

public class NotificationService(IHubContext<NotificationHub> hubContext) : INotificationService
{
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