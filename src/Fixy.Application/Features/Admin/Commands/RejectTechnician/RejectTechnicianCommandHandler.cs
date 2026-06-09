using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Resources;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fixy.Application.Features.Admin.Commands.RejectTechnician;

public sealed class RejectTechnicianCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, ILogger<RejectTechnicianCommandHandler> logger) : IRequestHandler<RejectTechnicianCommand, Result>
{
    public async Task<Result> Handle(RejectTechnicianCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Admin attempting to reject technician. TechnicianId: {TechnicianId}", request.TechnicianId);

        var currentUser = await currentUserService.GetCurrentUserAsync();

        if (currentUser == null)
        {
            logger.LogWarning("Reject technician failed — unauthorized, no current user resolved. TechnicianId: {TechnicianId}", request.TechnicianId);
            return Errors.Unauthorized;
        }

        var technician = await unitOfWork.Technicians.GetTableAsTracking().FirstOrDefaultAsync(x => x.Id == request.TechnicianId);

        if (technician == null)
        {
            logger.LogWarning("Reject technician failed — technician not found. TechnicianId: {TechnicianId}", request.TechnicianId);
            return Errors.TechnicianNotFound;
        }

        if (technician.Status == TechnicianStatus.Rejected)
        {
            logger.LogWarning("Reject technician skipped — already rejected. TechnicianId: {TechnicianId}, RejectedAt: {RejectedAt}", request.TechnicianId, technician.RejectedAt);
            return Errors.TechnicianAlreadyRejected;
        }

        technician.Status = TechnicianStatus.Rejected;
        technician.RejectionReason = request.Reason;
        technician.RejectedAt = DateTime.UtcNow;

        await unitOfWork.SaveChangesAsync();
        logger.LogInformation("Technician successfully rejected. TechnicianId: {TechnicianId}, AdminId: {AdminId}, Reason: {Reason}", request.TechnicianId, currentUser.Id, request.Reason);

        BackgroundJob.Enqueue<INotificationService>(x => x.SendFullNotificationAsync(
            technician,
            NotificationType.TechnicianRejected,
            SharedResourcesKeys.NotificationTechnicianRejectedTitle,
            SharedResourcesKeys.NotificationTechnicianRejectedBody,
            new Dictionary<string, string> { { "technicianId", technician.Id.ToString() } }
        ));

        return Result.Success();
    }
}
