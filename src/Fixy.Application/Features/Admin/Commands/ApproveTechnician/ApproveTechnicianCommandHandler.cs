using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Resources;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fixy.Application.Features.Admin.Commands.ApproveTechnician;

public class ApproveTechnicianCommandHandler(IUnitOfWork unitOfWork, ILogger<ApproveTechnicianCommandHandler> logger) : IRequestHandler<ApproveTechnicianCommand, Result>
{
    public async Task<Result> Handle(ApproveTechnicianCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Admin attempting to approve technician. TechnicianId: {TechnicianId}", request.TechnicianId);

        var technician = await unitOfWork.Technicians.GetTableAsTracking().FirstOrDefaultAsync(x => x.Id == request.TechnicianId);

        if (technician == null)
        {
            logger.LogWarning("Technician approval failed — not found. TechnicianId: {TechnicianId}", request.TechnicianId);
            return Errors.TechnicianNotFound;
        }

        if (technician.Status == TechnicianStatus.Approved)
        {
            logger.LogWarning("Technician approval skipped — already approved. TechnicianId: {TechnicianId}, CurrentStatus: {Status}", request.TechnicianId, technician.Status);
            return Errors.TechnicianAlreadyApproved;
        }

        technician.Status = TechnicianStatus.Approved;
        await unitOfWork.SaveChangesAsync();
        logger.LogInformation("Technician successfully approved. TechnicianId: {TechnicianId}", request.TechnicianId);

        BackgroundJob.Enqueue<INotificationService>(x => x.SendFullNotificationAsync(
            technician.Id,
            NotificationType.TechnicianApproved,
            SharedResourcesKeys.NotificationTechnicianApprovedTitle,
            SharedResourcesKeys.NotificationTechnicianApprovedBody,
            new Dictionary<string, string> { { "technicianId", technician.Id.ToString() } }
        ));

        return Result.Success();
    }
}
