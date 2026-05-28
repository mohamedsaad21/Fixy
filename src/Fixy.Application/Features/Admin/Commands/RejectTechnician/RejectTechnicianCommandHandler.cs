using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Resources;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Fixy.Application.Features.Admin.Commands.RejectTechnician;

public sealed class RejectTechnicianCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, INotificationService notificationService) : IRequestHandler<RejectTechnicianCommand, Result>
{
    public async Task<Result> Handle(RejectTechnicianCommand request, CancellationToken cancellationToken)
    {
        Log.Information("Admin attempting to reject technician. TechnicianId: {TechnicianId}", request.TechnicianId);

        var currentUser = await currentUserService.GetCurrentUserAsync();

        if (currentUser == null)
        {
            Log.Warning("Reject technician failed — unauthorized, no current user resolved. TechnicianId: {TechnicianId}", request.TechnicianId);
            return Errors.Unauthorized;
        }

        var technician = await unitOfWork.Technicians.GetTableAsTracking().FirstOrDefaultAsync(x => x.Id == request.TechnicianId);

        if (technician == null)
        {
            Log.Warning("Reject technician failed — technician not found. TechnicianId: {TechnicianId}", request.TechnicianId);
            return Errors.TechnicianNotFound;
        }

        if (technician.Status == TechnicianStatus.Rejected)
        {
            Log.Warning("Reject technician skipped — already rejected. TechnicianId: {TechnicianId}, RejectedAt: {RejectedAt}", request.TechnicianId, technician.RejectedAt);
            return Errors.TechnicianAlreadyRejected;
        }

        technician.Status = TechnicianStatus.Rejected;
        technician.RejectionReason = request.Reason;
        technician.RejectedAt = DateTime.UtcNow;

        await notificationService.SendFullNotificationAsync(
            technician,
            NotificationType.TechnicianRejected,
            SharedResourcesKeys.NotificationTechnicianRejectedTitle,
            SharedResourcesKeys.NotificationTechnicianRejectedBody
        );
        await unitOfWork.SaveChangesAsync();
        Log.Information("Technician successfully rejected. TechnicianId: {TechnicianId}, AdminId: {AdminId}, Reason: {Reason}", request.TechnicianId, currentUser.Id, request.Reason);
        return Result.Success();
    }
}
