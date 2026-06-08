using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Resources;
using Fixy.Domain.Entities;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fixy.Application.Features.Bookings.Commands.DisputeBookingCompletion;

public class DisputeBookingCompletionCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, INotificationService notificationService, ILogger<DisputeBookingCompletionCommandHandler> logger) : IRequestHandler<DisputeBookingCompletionCommand, Result>
{
    public async Task<Result> Handle(DisputeBookingCompletionCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Customer attempting to dispute booking completion. BookingId: {BookingId}", request.BookingId);

        var booking = await unitOfWork.Bookings.GetTableAsTracking()
            .Include(x => x.ServiceRequest).ThenInclude(x => x.Customer)
            .Include(x => x.Technician)
            .FirstOrDefaultAsync(x => x.Id == request.BookingId, cancellationToken);

        if (booking == null)
        {
            logger.LogWarning("Booking completion dispute failed — booking not found. BookingId: {BookingId}", request.BookingId);
            return Errors.BookingNotFound;
        }

        var currentCustomer = await currentUserService.GetCurrentUserAsync();

        if (booking.ServiceRequest.Customer.Id != currentCustomer.Id)
        {
            logger.LogWarning("Booking completion dispute failed — unauthorized customer. BookingId: {BookingId}, RequestingCustomerId: {RequestingCustomerId}", request.BookingId, currentCustomer.Id);
            return Errors.Unauthorized;
        }

        if (booking.Status != ServiceBookingStatus.AwaitingCustomerConfirmationForCompletion)
        {
            logger.LogWarning("Booking completion dispute failed — invalid booking state. BookingId: {BookingId}, CurrentStatus: {CurrentStatus}", request.BookingId, booking.Status);
            return Errors.BookingNotAwaitingConfirmation;
        }

        var existingDispute = await unitOfWork.Disputes.GetTableNoTracking()
            .FirstOrDefaultAsync(x => x.ServiceBookingId == request.BookingId, cancellationToken);

        if (existingDispute != null)
        {
            logger.LogWarning("Booking completion dispute failed — dispute already exists. BookingId: {BookingId}", request.BookingId);
            return Errors.DisputeAlreadyRaised;
        }

        var dispute = new Dispute
        {
            ServiceBookingId = booking.Id,
            RaiserId = currentCustomer.Id,
            Reason = request.Reason,
            DesiredResolution = request.DesiredResolution,
            Status = DisputeStatus.Open,
            CreatedAt = DateTimeOffset.UtcNow
        };

        booking.Status = ServiceBookingStatus.Disputed;

        await unitOfWork.Disputes.AddAsync(dispute);
        
        await notificationService.SendFullNotificationAsync(
            booking.Technician,
            NotificationType.DisputeRaised,
            SharedResourcesKeys.NotificationDisputeRaisedTitle,
            SharedResourcesKeys.NotificationDisputeRaisedBody
        );

        await unitOfWork.SaveChangesAsync();

        logger.LogInformation("Booking completion disputed by customer. BookingId: {BookingId}, CustomerId: {CustomerId}", request.BookingId, currentCustomer.Id);

        return Result.Success();
    }
}
