using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Resources;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Fixy.Application.Features.Admin.Commands.UnblockTechnician;

public sealed class UnblockTechnicianCommandHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork, INotificationService notificationService) : IRequestHandler<UnblockTechnicianCommand, Result>
{
    public async Task<Result> Handle(UnblockTechnicianCommand request, CancellationToken cancellationToken)
    {
        Log.Information("Admin attempting to unblock technician. TechnicianId: {TechnicianId}", request.TechnicianId);

        var currentUser = await currentUserService.GetCurrentUserAsync();

        if (currentUser == null)
        {
            Log.Warning("Unblock technician failed — unauthorized, no current user resolved. TechnicianId: {TechnicianId}", request.TechnicianId);
            return Errors.Unauthorized;
        }

        var technician = await unitOfWork.Technicians.GetTableAsTracking().FirstOrDefaultAsync(x => x.Id == request.TechnicianId);

        if (technician == null)
        {
            Log.Warning("Unblock technician failed — technician not found. TechnicianId: {TechnicianId}", request.TechnicianId);
            return Errors.TechnicianNotFound;
        }

        if (technician.Status == TechnicianStatus.Approved)
        {
            Log.Warning("Unblock technician skipped — technician is already approved/active. TechnicianId: {TechnicianId}", request.TechnicianId);
            return Errors.TechnicianAlreadyApproved;
        }

        technician.Status = TechnicianStatus.Approved;
        technician.BlockReason = null;
        technician.BlockedAt = null;
        technician.BlockedBy = null;

        await notificationService.SendFullNotificationAsync(
            technician,
            NotificationType.TechnicianUnblocked,
            SharedResourcesKeys.NotificationTechnicianUnblockedTitle,
            SharedResourcesKeys.NotificationTechnicianUnblockedBody
        );

        await unitOfWork.SaveChangesAsync();
        Log.Information("Technician successfully unblocked. TechnicianId: {TechnicianId}, AdminId: {AdminId}", request.TechnicianId, currentUser.Id);
        return Result.Success();
    }
}
