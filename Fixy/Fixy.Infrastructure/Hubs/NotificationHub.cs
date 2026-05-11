using Fixy.Application.Contracts.Services;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Infrastructure.Hubs;

[Authorize]
public class NotificationHub : Hub
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPresenceService _presenceService;
    public NotificationHub(IUnitOfWork unitOfWork, IPresenceService presenceService)
    {
        _unitOfWork = unitOfWork;
        _presenceService = presenceService;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst("uid")?.Value;
        if (!string.IsNullOrEmpty(userId))
        {
            var userGuid = Guid.Parse(userId);
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
            await _presenceService.SetOnlineAsync(userGuid, Context.ConnectionId);

            // Notify the other party in any open conversations
            var relatedUserIds = await GetRelatedUserIdsAsync(userGuid);

            foreach (var relatedId in relatedUserIds)
            {
                await Clients.Group($"user_{relatedId}")
                    .SendAsync("UserOnline", new { UserId = userGuid });
            }
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.FindFirst("uid")?.Value;
        if (!string.IsNullOrEmpty(userId))
        {
            var userGuid = Guid.Parse(userId);
            await _presenceService.SetOfflineAsync(Context.ConnectionId);

            var isStillOnline = await _presenceService.IsOnlineAsync(userGuid);
            if (!isStillOnline)
            {
                var relatedUserIds = await GetRelatedUserIdsAsync(userGuid);
                foreach (var relatedId in relatedUserIds)
                {
                    await Clients.Group($"user_{relatedId}")
                        .SendAsync("UserOffline", new { UserId = userGuid });
                }
            }
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");
        }
        await base.OnDisconnectedAsync(exception);
    }

    private async Task<List<Guid>> GetRelatedUserIdsAsync(Guid userId)
    {
        return await _unitOfWork.Bookings
            .GetTableNoTracking()
            .Include(x => x.ServiceRequest)
            .Where(x => (x.TechnicianId == userId || x.ServiceRequest.CustomerId == userId)
            && x.Status != ServiceBookingStatus.CancelledByCustomer
            && x.Status != ServiceBookingStatus.CancelledByTechnician
            && x.Status != ServiceBookingStatus.Completed)
            .Select(x => x.TechnicianId == userId
                ? x.ServiceRequest.CustomerId
                : x.TechnicianId)
            .Distinct()
            .ToListAsync();
    }
}