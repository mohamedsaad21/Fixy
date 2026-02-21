using Fixy.Application.Abstracts;
using Fixy.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Fixy.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationService(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendNotificationToUserAsync(Guid userId, object payload, CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients
            .Group($"user_{userId}")
            .SendAsync("ReceiveNotification", payload, cancellationToken);
    }
}