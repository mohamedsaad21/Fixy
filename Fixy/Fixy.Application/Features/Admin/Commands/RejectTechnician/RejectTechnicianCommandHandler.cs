using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
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

        var payload = new
        {
            type = "TECHNICIAN_REJECTED",
            message = "Application Rejected",
            Message = $"Reason: {request.Reason}",
            CreatedAt = DateTime.UtcNow
        };

        await notificationService.SaveNotificationAsync(technician.Id, payload.type, payload);

        await unitOfWork.SaveChangesAsync();

        await notificationService.SendNotificationToUserAsync(technician.Id, payload);

        if (!string.IsNullOrEmpty(technician.FcmToken))
        {
            await notificationService.SendPushNotificationAsync(
                fcmToken: technician.FcmToken,
                title: "Account Rejected",
                body: "Your account has been rejected. Please review the reason and update your information.",
                data: new Dictionary<string, string>
                {
                    { "type", "TECHNICIAN_REJECTED" },
                    { "reason", technician.RejectionReason ?? "Not specified" },
                    { "createdAt", DateTime.UtcNow.ToString("O") }
                }
            );
        }

        return Result.Success();
    }
}
