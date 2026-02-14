using Fixy.Application.Abstracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Fixy.Api.Hubs;

[Authorize]
public class NotificationHub : Hub
{
    private readonly ILogger<NotificationHub> _logger;
    private readonly ICurrentUserService _currentUserService;
    public NotificationHub(ILogger<NotificationHub> logger, ICurrentUserService currentUserService)
    {
        _logger = logger;
        _currentUserService = currentUserService;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = _currentUserService.GetCurrentUserId();

        if (userId != null)
        {
            // Add user to their personal group
            await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");

            // Add to role-based groups
            var role = await _currentUserService.GetCurrentUserRoleAsync();
            if (!string.IsNullOrEmpty(role))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, role);

                // Special handling for admins
                if (role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
                }
            }

            _logger.LogInformation(
                $"User {userId} ({role}) connected. ConnectionId: {Context.ConnectionId}"
            );

            // Notify user of successful connection
            await Clients.Caller.SendAsync("Connected", new
            {
                userId,
                connectionId = Context.ConnectionId,
                timestamp = DateTime.UtcNow
            });
        }
        else
        {
            _logger.LogWarning($"Anonymous connection attempt: {Context.ConnectionId}");
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var userId = _currentUserService.GetCurrentUserId();

        if (userId != null)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{userId}");

            var role = await _currentUserService.GetCurrentUserRoleAsync();
            if (!string.IsNullOrEmpty(role))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, role);

                if (role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Admins");
                }
            }

            _logger.LogInformation($"User {userId} disconnected. ConnectionId: {Context.ConnectionId}");
        }

        if (exception != null)
        {
            _logger.LogError(exception, "Connection disconnected with error");
        }

        await base.OnDisconnectedAsync(exception);
    }

    // Client can call this to join custom groups (e.g., for specific service areas)
    public async Task JoinGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        _logger.LogInformation($"User joined group: {groupName}");
    }

    public async Task LeaveGroup(string groupName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        _logger.LogInformation($"User left group: {groupName}");
    }

    // Client can ping to check connection
    public async Task Ping()
    {
        await Clients.Caller.SendAsync("Pong", DateTime.UtcNow);
    }
}