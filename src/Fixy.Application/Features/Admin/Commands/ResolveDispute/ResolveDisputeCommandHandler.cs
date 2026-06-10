using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Resources;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fixy.Application.Features.Admin.Commands.ResolveDispute;

public class ResolveDisputeCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, ILogger<ResolveDisputeCommandHandler> logger) : IRequestHandler<ResolveDisputeCommand, Result>
{
    public async Task<Result> Handle(ResolveDisputeCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Admin attempting to resolve dispute. DisputeId: {DisputeId}", request.DisputeId);

        var dispute = await unitOfWork.Disputes.GetTableAsTracking()
            .Include(x => x.ServiceBooking).ThenInclude(x => x.ServiceRequest).ThenInclude(x => x.Customer)
            .Include(x => x.ServiceBooking).ThenInclude(x => x.Technician)
            .FirstOrDefaultAsync(x => x.Id == request.DisputeId, cancellationToken);

        if (dispute == null)
        {
            logger.LogWarning("Resolve dispute failed — dispute not found. DisputeId: {DisputeId}", request.DisputeId);
            return Errors.DisputeNotFound;
        }

        if (dispute.Status == DisputeStatus.ResolvedInFavorOfCustomer || 
            dispute.Status == DisputeStatus.ResolvedInFavorOfTechnician || 
            dispute.Status == DisputeStatus.Dismissed)
        {
            logger.LogWarning("Resolve dispute failed — dispute already resolved. DisputeId: {DisputeId}, CurrentStatus: {CurrentStatus}", request.DisputeId, dispute.Status);
            return Errors.DisputeAlreadyResolved;
        }

        if (request.Outcome == DisputeStatus.Open || request.Outcome == DisputeStatus.UnderReview)
        {
             return Errors.InvalidDisputeStatus;
        }

        var currentAdmin = await currentUserService.GetCurrentUserAsync();

        dispute.Status = request.Outcome;
        dispute.ResolutionOutcome = request.ResolutionOutcome;
        dispute.ResolvedAt = DateTimeOffset.UtcNow;
        dispute.ResolverId = currentAdmin.Id;

        if (request.Outcome == DisputeStatus.ResolvedInFavorOfCustomer || request.Outcome == DisputeStatus.Dismissed)
        {
            // Usually, favor of customer implies no payment to technician or refund
            // In our system, if completed but disputed before payment, favor of customer
            // means booking is cancelled or no payment required. 
            // We'll revert status to CancelledByCustomer (which waives payment) or CustomerCompleted depending on business logic.
            // Let's set it to CancelledByCustomer for simplicity to stop payment flow.
            dispute.ServiceBooking.Status = ServiceBookingStatus.CancelledByCustomer;
            dispute.ServiceBooking.CancelledAt = DateTimeOffset.UtcNow;
            dispute.ServiceBooking.CancelledById = currentAdmin.Id;
            dispute.ServiceBooking.CancellationNote = $"Dispute resolved in favor of customer/dismissed. Admin note: {request.ResolutionOutcome}";
        }
        else if (request.Outcome == DisputeStatus.ResolvedInFavorOfTechnician)
        {
            // Favor of technician means work was done, push to AwaitingPayment
            dispute.ServiceBooking.Status = ServiceBookingStatus.AwaitingPayment;
        }

        await unitOfWork.SaveChangesAsync();
        logger.LogInformation("Dispute resolved by admin. DisputeId: {DisputeId}, AdminId: {AdminId}, Outcome: {Outcome}", request.DisputeId, currentAdmin.Id, request.Outcome);

        BackgroundJob.Enqueue<INotificationService>(x => x.SendFullNotificationAsync(
            dispute.ServiceBooking.ServiceRequest.CustomerId,
            NotificationType.DisputeResolved,
            SharedResourcesKeys.NotificationDisputeResolvedTitle,
            SharedResourcesKeys.NotificationDisputeResolvedBody,
            new Dictionary<string, string> { { "bookingId", dispute.ServiceBookingId.ToString() } }
        ));

        BackgroundJob.Enqueue<INotificationService>(x => x.SendFullNotificationAsync(
            dispute.ServiceBooking.TechnicianId,
            NotificationType.DisputeResolved,
            SharedResourcesKeys.NotificationDisputeResolvedTitle,
            SharedResourcesKeys.NotificationDisputeResolvedBody,
            new Dictionary<string, string> { { "bookingId", dispute.ServiceBookingId.ToString() } }
        ));

        return Result.Success();
    }
}
