using Fixy.Api.Hubs;
using Fixy.Application.Abstracts;
using Microsoft.AspNetCore.SignalR;

namespace Fixy.Api.Services;

public class SignalRNotificationSender : IRealtimeNotificationSender
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly ILogger<SignalRNotificationSender> _logger;

    public SignalRNotificationSender(
        IHubContext<NotificationHub> hubContext,
        ILogger<SignalRNotificationSender> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task SendToUserAsync(Guid userId, string method, object data)
    {
        try
        {
            await _hubContext.Clients
                .Group($"User_{userId}")
                .SendAsync(method, data);

            _logger.LogInformation($"Notification sent to user {userId} via {method}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to send notification to user {userId}");
        }
    }

    public async Task SendToGroupAsync(string groupName, string method, object data)
    {
        try
        {
            await _hubContext.Clients
                .Group(groupName)
                .SendAsync(method, data);

            _logger.LogInformation($"Notification sent to group {groupName} via {method}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to send notification to group {groupName}");
        }
    }

    public async Task SendToAllAsync(string method, object data)
    {
        try
        {
            await _hubContext.Clients
                .All
                .SendAsync(method, data);

            _logger.LogInformation($"Notification broadcast to all users via {method}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to broadcast notification");
        }
    }

    public async Task SendToUsersAsync(IEnumerable<Guid> userIds, string method, object data)
    {
        try
        {
            var groups = userIds.Select(id => $"User_{id}");
            await _hubContext.Clients
                .Groups(groups)
                .SendAsync(method, data);

            _logger.LogInformation($"Notification sent to {userIds.Count()} users via {method}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send notification to multiple users");
        }
    }
}
