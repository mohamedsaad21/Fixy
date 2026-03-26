using Fixy.Application.Abstracts;
using Fixy.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Fixy.Infrastructure.Services;

public class NotificationService(IHubContext<NotificationHub> hubContext) : INotificationService
{
    public async Task SendNotificationToUserAsync(Guid userId, object payload, CancellationToken cancellationToken = default)
    {
        await hubContext.Clients
            .Group($"user_{userId}")
            .SendAsync("ReceiveNotification", payload, cancellationToken);
    }
}