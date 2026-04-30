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

        var payload = new
        {
            title = localizer[SharedResourcesKeys.NotificationTechnicianApprovedTitle],
            message = localizer[SharedResourcesKeys.NotificationTechnicianApprovedBody],
            type = NotificationType.TechnicianApproved.ToString(),
            createdAt = DateTime.UtcNow
        };

        await notificationService.SaveNotificationAsync(technician.Id, NotificationType.TechnicianApproved,
            SharedResourcesKeys.NotificationTechnicianApprovedTitle,
            SharedResourcesKeys.NotificationTechnicianApprovedBody);

        await unitOfWork.SaveChangesAsync();

        await notificationService.SendNotificationToUserAsync(technician.Id, payload);

        if (!string.IsNullOrEmpty(technician.FcmToken))
        {
            await notificationService.SendPushNotificationAsync(
                fcmToken: technician.FcmToken,
                title: "Account Approved",
                body: "Congratulations! Your account has been approved. You can now start receiving service requests.",
                data: new Dictionary<string, string>
                {
                    { "type", "TECHNICIAN_APPROVED" },
                    { "technicianId", technician.Id.ToString() },
                    { "approvedAt", DateTime.UtcNow.ToString("O") }
                }
            );
        }

        return Result.Success();
    }
}
