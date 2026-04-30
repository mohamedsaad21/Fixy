using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Admin.Commands.BlockTecnhnician;

public sealed class BlockTecnhnicianCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, INotificationService notificationService) : IRequestHandler<BlockTecnhnicianCommand, Result>
{
    public async Task<Result> Handle(BlockTecnhnicianCommand request, CancellationToken cancellationToken)
    {
        var currentUser = await currentUserService.GetCurrentUserAsync();

        if (currentUser == null)
            return Errors.Unauthorized;

        var technician = await unitOfWork.Technicians.GetTableAsTracking().FirstOrDefaultAsync(x => x.Id == request.TechnicianId);

        if (technician == null)
            return Errors.TechnicianNotFound;

        if (technician.Status == TechnicianStatus.Blocked)
            return Errors.TechnicianAlreadyBlocked;

        technician.Status = TechnicianStatus.Blocked;
        technician.BlockReason = request.Reason;
        technician.BlockedAt = DateTime.UtcNow;
        technician.BlockedBy = currentUser.Id;

        var payload = new
        {
            type = NotificationType.TechnicianBlocked,
            Title = "Account Blocked",
            Message = $"Reason: {request.Reason}",
            CreatedAt = DateTime.UtcNow
        };

        //await notificationService.SaveNotificationAsync(technician.Id, payload.type, payload);

        await unitOfWork.SaveChangesAsync();

        // Notify technician
        await notificationService.SendNotificationToUserAsync(technician.Id, payload);

        if (!string.IsNullOrEmpty(technician.FcmToken))
        {
            await notificationService.SendPushNotificationAsync(
                fcmToken: technician.FcmToken,
                title: "Account Blocked",
                body: "Your account has been blocked. Contact support for more details.",
                data: new Dictionary<string, string>
                {
                    { "type", "TECHNICIAN_BLOCKED" },
                    { "reason", request.Reason },
                    { "createdAt", DateTime.UtcNow.ToString("O") }
                }
            );
        }
        return Result.Success();
    }
}
