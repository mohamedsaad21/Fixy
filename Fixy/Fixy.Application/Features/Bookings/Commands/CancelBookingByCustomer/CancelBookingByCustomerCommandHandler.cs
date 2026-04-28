using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Bookings.Commands.CancelBookingByCustomer;

public sealed class CancelBookingByCustomerCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IBookingService bookingService, INotificationService notificationService) : IRequestHandler<CancelBookingByCustomerCommand, Result>
{
    public async Task<Result> Handle(CancelBookingByCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await currentUserService.GetCurrentUserAsync();

        if (customer == null)
            return Errors.Unauthorized;

        var booking = await unitOfWork.Bookings.GetTableAsTracking().Include(x => x.ServiceRequest)
            .ThenInclude(x => x.Customer).Include(x => x.Technician)
            .FirstOrDefaultAsync(x => x.Id == request.BookingId);

        if (booking == null)
            return Errors.BookingNotFound;

        if (booking.ServiceRequest.CustomerId != customer.Id)
            return Errors.Unauthorized;

        if (booking.Status == ServiceBookingStatus.Cancelled)
            return Errors.AlreadyCancelled;

        if(booking.Status != ServiceBookingStatus.InProgress && booking.Status != ServiceBookingStatus.AwaitingPriceChangeApproval)
            return Errors.CannotCancelAtThisStage;


        booking.CustomerCancellationReason = request.Reason;
        booking.CancellationNote = request.Note;
        booking.ServiceRequest.Customer.CancelledBookings += 1;
        booking.ServiceRequest.Customer.CancellationRate =
            (double)booking.ServiceRequest.Customer.CancelledBookings / booking.ServiceRequest.Customer.TotalBookings * 100;
        await bookingService.CancelBookingAsync(booking, customer.Id);
        
        // send notification to technician
        var techniain = booking.Technician;

        var payload = new
        {
            type = "BOOKING_CANCELLED",
            Message = "Customer cancelled the booking",
            CreatedAt = DateTime.UtcNow
        };

        await notificationService.SaveNotificationAsync(techniain.Id, payload.type, payload);

        await unitOfWork.SaveChangesAsync();

        if (techniain != null)
        {
            await notificationService.SendNotificationToUserAsync(techniain.Id, payload);
            if (!string.IsNullOrEmpty(techniain.FcmToken))
            {
                await notificationService.SendPushNotificationAsync(
                    fcmToken: techniain.FcmToken,
                    title: "Booking Cancelled",
                    body: "Customer cancelled the booking",
                    data: new Dictionary<string, string>
                    {
                    { "type", "BOOKING_CANCELLED" },
                    { "bookingId", booking.Id.ToString() },
                    { "createdAt", DateTime.UtcNow.ToString("O") }
                    }
                );
            }
        }        
        return Result.Success();
    }
}
