using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Resources;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Serilog;

namespace Fixy.Application.Features.Admin.Commands.ApproveTechnician;

public class ApproveTechnicianCommandHandler(IUnitOfWork unitOfWork, INotificationService notificationService, IStringLocalizer<SharedResources> localizer) : IRequestHandler<ApproveTechnicianCommand, Result>
{
    public async Task<Result> Handle(ApproveTechnicianCommand request, CancellationToken cancellationToken)
    {
        Log.Information("Admin attempting to approve technician. TechnicianId: {TechnicianId}", request.TechnicianId);

        var technician = await unitOfWork.Technicians.GetTableAsTracking().FirstOrDefaultAsync(x => x.Id == request.TechnicianId);

        if (technician == null)
        {
            Log.Warning("Technician approval failed — not found. TechnicianId: {TechnicianId}", request.TechnicianId);
            return Errors.TechnicianNotFound;
        }

        if (technician.Status == TechnicianStatus.Approved)
        {
            Log.Warning("Technician approval skipped — already approved. TechnicianId: {TechnicianId}, CurrentStatus: {Status}", request.TechnicianId, technician.Status);
            return Errors.TechnicianAlreadyApproved;
        }

        technician.Status = TechnicianStatus.Approved;

        await notificationService.SendFullNotificationAsync(
            technician,
            NotificationType.TechnicianApproved,
            SharedResourcesKeys.NotificationTechnicianApprovedTitle,
            SharedResourcesKeys.NotificationTechnicianApprovedBody
        );
        await unitOfWork.SaveChangesAsync();
        Log.Information("Technician successfully approved. TechnicianId: {TechnicianId}", request.TechnicianId);
        return Result.Success();
    }
}
