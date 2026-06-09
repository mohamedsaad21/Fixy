using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Resources;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fixy.Application.Features.Bookings.Commands.CancelBookingByCustomer;

public sealed class CancelBookingByCustomerCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService,
    IBookingService bookingService, ILogger<CancelBookingByCustomerCommandHandler>logger) : IRequestHandler<CancelBookingByCustomerCommand, Result>
{
    public async Task<Result> Handle(CancelBookingByCustomerCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Customer attempting to cancel booking. BookingId: {BookingId}", request.BookingId);
        
        var customer = await currentUserService.GetCurrentUserAsync();

        if (customer == null)
        {
            logger.LogWarning("Booking cancellation failed — unauthorized, no current user resolved. BookingId: {BookingId}", request.BookingId);
            return Errors.Unauthorized;
        }

        var booking = await unitOfWork.Bookings.GetTableAsTracking().Include(x => x.ServiceRequest)
            .ThenInclude(x => x.Customer).Include(x => x.Technician)
            .FirstOrDefaultAsync(x => x.Id == request.BookingId);

        if (booking == null)
        {
            logger.LogWarning("Booking cancellation failed — booking not found. BookingId: {BookingId}, CustomerId: {CustomerId}", request.BookingId, customer.Id);
            return Errors.BookingNotFound;
        }

        if (booking.ServiceRequest.CustomerId != customer.Id)
        {
            logger.LogWarning("Booking cancellation failed — unauthorized customer. BookingId: {BookingId}, BookingCustomerId: {BookingCustomerId}, RequestingCustomerId: {RequestingCustomerId}", request.BookingId, booking.ServiceRequest.CustomerId, customer.Id);
            return Errors.Unauthorized;
        }

        if(booking.Status != ServiceBookingStatus.InProgress && booking.Status != ServiceBookingStatus.AwaitingPriceChangeApproval)
        {
            logger.LogWarning("Booking cancellation failed — invalid booking state. BookingId: {BookingId}, CurrentStatus: {CurrentStatus}", request.BookingId, booking.Status);
            return Errors.CannotCancelAtThisStage;
        }
        var previousCancellationRate = booking.CustomerCancellationReason;
        booking.ServiceRequest.Customer.CancelledBookings += 1;        
        customer.CancellationRate = customer.TotalBookings > 0? (double)customer.CancelledBookings / customer.TotalBookings * 100 : 0;
        booking.CustomerCancellationReason = request.Reason;
        booking.CancellationNote = request.Note;

        await bookingService.CancelBookingByCustomerAsync(booking, customer.Id, request.ReopenServiceRequest);
        
        // send notification to technician
        var technician = booking.Technician;

        await unitOfWork.SaveChangesAsync();
        logger.LogInformation("Booking cancelled by customer successfully. BookingId: {BookingId}, CustomerId: {CustomerId}, TechnicianId: {TechnicianId}, Reason: {Reason}, ReopenServiceRequest: {ReopenServiceRequest}, PreviousCancellationRate: {PreviousCancellationRate}, NewCancellationRate: {NewCancellationRate}",
            request.BookingId, customer.Id, booking.Technician.Id, request.Reason, request.ReopenServiceRequest,
            previousCancellationRate, customer.CancellationRate);

        BackgroundJob.Enqueue<INotificationService>(x => x.SendFullNotificationAsync(
            technician,
            NotificationType.BookingCancelledByCustomer,
            SharedResourcesKeys.NotificationBookingCancelledByCustomerTitle,
            SharedResourcesKeys.NotificationBookingCancelledByCustomerBody,
            new Dictionary<string, string> { { "bookingId", booking.Id.ToString() } }
        ));

        return Result.Success();
    }
}
