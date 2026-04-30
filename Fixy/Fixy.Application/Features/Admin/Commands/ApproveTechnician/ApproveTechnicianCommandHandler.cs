using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Resources;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Features.Admin.Commands.ApproveTechnician;

public class ApproveTechnicianCommandHandler(IUnitOfWork unitOfWork, INotificationService notificationService, IStringLocalizer<SharedResources> localizer) : IRequestHandler<ApproveTechnicianCommand, Result>
{
    public async Task<Result> Handle(ApproveTechnicianCommand request, CancellationToken cancellationToken)
    {
        var technician = await unitOfWork.Technicians.GetTableAsTracking().FirstOrDefaultAsync(x => x.Id == request.TechnicianId);

        if (technician == null)
            return Errors.TechnicianNotFound;

        if (technician.Status == TechnicianStatus.Approved)
            return Errors.TechnicianAlreadyApproved;

        technician.Status = TechnicianStatus.Approved;

        await notificationService.SendFullNotificationAsync(
            technician,
            NotificationType.TechnicianApproved,
            SharedResourcesKeys.NotificationTechnicianApprovedTitle,
            SharedResourcesKeys.NotificationTechnicianApprovedBody
        );
        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
