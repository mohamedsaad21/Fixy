using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Resources;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Admin.Commands.UnblockTechnician;

public sealed class UnblockTechnicianCommandHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork, INotificationService notificationService) : IRequestHandler<UnblockTechnicianCommand, Result>
{
    public async Task<Result> Handle(UnblockTechnicianCommand request, CancellationToken cancellationToken)
    {
        var currentUser = await currentUserService.GetCurrentUserAsync();

        if (currentUser == null)
            return Errors.Unauthorized;

        var technician = await unitOfWork.Technicians.GetTableAsTracking().FirstOrDefaultAsync(x => x.Id == request.TechnicianId);

        if (technician == null)
            return Errors.TechnicianNotFound;

        if (technician.Status == TechnicianStatus.Approved)
            return Errors.TechnicianAlreadyApproved;

        technician.Status = TechnicianStatus.Approved;
        technician.BlockReason = null;
        technician.BlockedAt = null;
        technician.BlockedBy = null;

        await notificationService.SendFullNotificationAsync(
            technician,
            NotificationType.TechnicianApproved,
            SharedResourcesKeys.NotificationTechnicianUnblockedTitle,
            SharedResourcesKeys.NotificationTechnicianUnblockedBody
        );

        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
