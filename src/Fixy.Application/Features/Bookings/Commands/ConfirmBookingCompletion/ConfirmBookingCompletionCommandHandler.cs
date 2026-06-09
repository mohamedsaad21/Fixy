using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Fixy.Application.Resources;

namespace Fixy.Application.Features.Bookings.Commands.ConfirmBookingCompletion;

public class ConfirmBookingCompletionCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, ILogger<ConfirmBookingCompletionCommandHandler>logger) : IRequestHandler<ConfirmBookingCompletionCommand, Result>
{
    public async Task<Result> Handle(ConfirmBookingCompletionCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Customer attempting to confirm booking completion. BookingId: {BookingId}", request.BookingId);
        
        var booking = await unitOfWork.Bookings.GetTableAsTracking()
            .Include(x => x.ServiceRequest).ThenInclude(x => x.Customer)
            .Include(x => x.Technician)
            .FirstOrDefaultAsync(x => x.Id == request.BookingId);

        if (booking == null)
        {
            logger.LogWarning("Booking completion confirmation failed — booking not found. BookingId: {BookingId}", request.BookingId);
            return Errors.BookingNotFound;
        }

        var currentCustomer = await currentUserService.GetCurrentUserAsync();

        if (booking.ServiceRequest.Customer.Id != currentCustomer.Id)
        {
            logger.LogWarning("Booking completion confirmation failed — unauthorized customer. BookingId: {BookingId}, BookingCustomerId: {BookingCustomerId}, RequestingCustomerId: {RequestingCustomerId}", request.BookingId, booking.ServiceRequest.Customer.Id, currentCustomer.Id);
            return Errors.Unauthorized;
        }

        if (booking.Status != ServiceBookingStatus.AwaitingCustomerConfirmationForCompletion)
        {
            logger.LogWarning("Booking completion confirmation failed — invalid booking state. BookingId: {BookingId}, CurrentStatus: {CurrentStatus}", request.BookingId, booking.Status);
            return Errors.InvalidBookingState;
        }

        booking.Status = ServiceBookingStatus.AwaitingPayment;
        booking.IsCustomerConfirmed = true;
        booking.CustomerConfirmedAt = DateTime.UtcNow;

        await unitOfWork.SaveChangesAsync();

        logger.LogInformation("Booking completion confirmed by customer — awaiting payment. BookingId: {BookingId}, CustomerId: {CustomerId}, AgreedPrice: {AgreedPrice}, ConfirmedAt: {ConfirmedAt}",
            request.BookingId, currentCustomer.Id, booking.AgreedPrice, booking.CustomerConfirmedAt);

        BackgroundJob.Enqueue<INotificationService>(x => x.SendFullNotificationAsync(
            booking.Technician,
            NotificationType.BookingCompleted,
            SharedResourcesKeys.NotificationBookingCompletedTitle,
            SharedResourcesKeys.NotificationBookingCompletedBody,
            new Dictionary<string, string> { { "bookingId", booking.Id.ToString() } }
        ));

        return Result.Success();
    }
}