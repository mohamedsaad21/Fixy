using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Resources;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Admin.Commands.RejectTechnician;

public sealed class RejectTechnicianCommandHandler(IUnitOfWork unitOfWork, INotificationService notificationService) : IRequestHandler<RejectTechnicianCommand, Result>
{
    public async Task<Result> Handle(RejectTechnicianCommand request, CancellationToken cancellationToken)
    {
        var technician = await unitOfWork.Technicians.GetTableAsTracking().FirstOrDefaultAsync(x => x.Id == request.TechnicianId);

        if (technician == null)
            return Errors.TechnicianNotFound;

        if (technician.Status == TechnicianStatus.Rejected)
            return Errors.TechnicianAlreadyRejected;

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
        return Result.Success();
    }
}
