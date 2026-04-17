using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Entities;
using Fixy.Domain.Entities.Identity;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Text.Json;

namespace Fixy.Application.Features.Notifications.Commands.SendNotification;

public sealed class SendNotificationCommandHandler : IRequestHandler<SendNotificationCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly INotificationService _notificationService;

    public SendNotificationCommandHandler(IUnitOfWork unitOfWork, INotificationService notificationService, UserManager<ApplicationUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
        _userManager = userManager;
    }

    public async Task<Result<bool>> Handle(SendNotificationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            Log.Information($"Sending notification to user: {request.UserId}, Type: {request.Type}");

            // 1. Verify user exists
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == request.UserId);

            if (user == null)
            {
                Log.Warning($"User not found: {request.UserId}");
                return Errors.UserNotFound;
            }

            // 2. Save notification to database
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Type = request.Type,
                Data = JsonSerializer.Serialize(request.Data),
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Notifications.AddAsync(notification);
            await _unitOfWork.SaveChangesAsync();

            Log.Information($"Notification saved to database: {notification.Id}");

            // 3. Send real-time notification via SignalR
            await _notificationService.SendNotificationToUserAsync(request.UserId, new
            {
                id = notification.Id,
                type = request.Type,
                data = request.Data,
                createdAt = notification.CreatedAt
            }, cancellationToken);

            Log.Information($"Real-time notification sent to user: {request.UserId}");

            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Error sending notification to user: {request.UserId}");
            return Errors.NotificationSendFailed;
        }
    }
}
