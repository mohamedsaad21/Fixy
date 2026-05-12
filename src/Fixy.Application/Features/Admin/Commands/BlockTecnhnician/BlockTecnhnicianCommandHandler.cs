using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Resources;
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

        await notificationService.SendFullNotificationAsync(
            technician,
            NotificationType.TechnicianBlocked,
            SharedResourcesKeys.NotificationTechnicianBlockedTitle,
            SharedResourcesKeys.NotificationTechnicianBlockedBody
        );
        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
