using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Resources;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fixy.Application.Features.Admin.Commands.UnblockTechnician;

public sealed class UnblockTechnicianCommandHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork, ILogger<UnblockTechnicianCommandHandler> logger) : IRequestHandler<UnblockTechnicianCommand, Result>
{
    public async Task<Result> Handle(UnblockTechnicianCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Admin attempting to unblock technician. TechnicianId: {TechnicianId}", request.TechnicianId);

        var currentUser = await currentUserService.GetCurrentUserAsync();

        if (currentUser == null)
        {
            logger.LogWarning("Unblock technician failed — unauthorized, no current user resolved. TechnicianId: {TechnicianId}", request.TechnicianId);
            return Errors.Unauthorized;
        }

        var technician = await unitOfWork.Technicians.GetTableAsTracking().FirstOrDefaultAsync(x => x.Id == request.TechnicianId);

        if (technician == null)
        {
            logger.LogWarning("Unblock technician failed — technician not found. TechnicianId: {TechnicianId}", request.TechnicianId);
            return Errors.TechnicianNotFound;
        }

        if (technician.Status == TechnicianStatus.Approved)
        {
            logger.LogWarning("Unblock technician skipped — technician is already approved/active. TechnicianId: {TechnicianId}", request.TechnicianId);
            return Errors.TechnicianAlreadyApproved;
        }

        technician.Status = TechnicianStatus.Approved;
        technician.BlockReason = null;
        technician.BlockedAt = null;
        technician.BlockedBy = null;

        await unitOfWork.SaveChangesAsync();
        logger.LogInformation("Technician successfully unblocked. TechnicianId: {TechnicianId}, AdminId: {AdminId}", request.TechnicianId, currentUser.Id);

        BackgroundJob.Enqueue<INotificationService>(x => x.SendFullNotificationAsync(
            technician.Id,
            NotificationType.TechnicianUnblocked,
            SharedResourcesKeys.NotificationTechnicianUnblockedTitle,
            SharedResourcesKeys.NotificationTechnicianUnblockedBody
        ));

        return Result.Success();
    }
}
