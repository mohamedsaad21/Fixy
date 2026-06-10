using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Resources;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fixy.Application.Features.Admin.Commands.BlockTecnhnician;

public sealed class BlockTecnhnicianCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, ILogger<BlockTecnhnicianCommandHandler> logger) : IRequestHandler<BlockTecnhnicianCommand, Result>
{
    public async Task<Result> Handle(BlockTecnhnicianCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Admin attempting to block technician. TechnicianId: {TechnicianId}", request.TechnicianId);
                
        var currentUser = await currentUserService.GetCurrentUserAsync();

        if (currentUser == null)
        {
            logger.LogWarning("Block technician failed — unauthorized, no current user resolved. TechnicianId: {TechnicianId}", request.TechnicianId);
            return Errors.Unauthorized;
        }

        var technician = await unitOfWork.Technicians.GetTableAsTracking().FirstOrDefaultAsync(x => x.Id == request.TechnicianId);

        if (technician == null)
        {
            logger.LogWarning("Block technician failed — technician not found. TechnicianId: {TechnicianId}", request.TechnicianId);
            return Errors.TechnicianNotFound;
        }

        if (technician.Status == TechnicianStatus.Blocked)
        {
            logger.LogWarning("Block technician skipped — already blocked. TechnicianId: {TechnicianId}, BlockedBy: {BlockedBy}", request.TechnicianId, technician.BlockedBy);
            return Errors.TechnicianAlreadyBlocked;
        }

        technician.Status = TechnicianStatus.Blocked;
        technician.BlockReason = request.Reason;
        technician.BlockedAt = DateTime.UtcNow;
        technician.BlockedBy = currentUser.Id;

        await unitOfWork.SaveChangesAsync();
        logger.LogInformation("Technician successfully blocked. TechnicianId: {TechnicianId}, AdminId: {AdminId}, Reason: {Reason}", request.TechnicianId, currentUser.Id, request.Reason);

        BackgroundJob.Enqueue<INotificationService>(x => x.SendFullNotificationAsync(
            technician.Id,
            NotificationType.TechnicianBlocked,
            SharedResourcesKeys.NotificationTechnicianBlockedTitle,
            SharedResourcesKeys.NotificationTechnicianBlockedBody,
            new Dictionary<string, string> { { "technicianId", technician.Id.ToString() } }
        ));

        return Result.Success();
    }
}
