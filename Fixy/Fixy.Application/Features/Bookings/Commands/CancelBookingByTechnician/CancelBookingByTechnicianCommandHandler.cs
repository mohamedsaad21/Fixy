using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Bookings.Commands.CancelBookingByTechnician;

public sealed class CancelBookingByTechnicianCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IBookingService bookingService, INotificationService notificationService) : IRequestHandler<CancelBookingByTechnicianCommand, Result>
{
    public async Task<Result> Handle(CancelBookingByTechnicianCommand request, CancellationToken cancellationToken)
    {
        var technician = await currentUserService.GetCurrentUserAsync();

        if (technician == null)
            return Errors.Unauthorized;

        var booking = await unitOfWork.Bookings.GetTableAsTracking().Include(x => x.ServiceRequest).ThenInclude(x => x.Customer)
            .Include(X => X.Technician)
            .FirstOrDefaultAsync(x => x.Id == request.BookingId);

        if (booking == null)
            return Errors.BookingNotFound;

        if (booking.TechnicianId != technician.Id)
            return Errors.Unauthorized;

        if (booking.Status == ServiceBookingStatus.Cancelled)
            return Errors.AlreadyCancelled;

        if(booking.Status != ServiceBookingStatus.InProgress && booking.Status != ServiceBookingStatus.AwaitingPriceChangeApproval)
            return Errors.CannotCancelAtThisStage;

        booking.TechnicianCancellationReason = request.Reason;
        booking.CancellationNote = request.Notes;
        booking.Technician.CancelledBookings += 1;
        booking.Technician.CancellationRate =
            (double)booking.Technician.CancelledBookings / booking.Technician.TotalBookings * 100;
        await bookingService.CancelBookingAsync(booking, technician.Id);
        // send notification to other user
        var customer = booking.ServiceRequest.Customer;

        var payload = new
        {
            type = "BOOKING_CANCELLED",
            Message = "Technician cancelled the booking",
            CreatedAt = DateTime.UtcNow
        };

        await notificationService.SaveNotificationAsync(customer.Id, payload.type, payload);

        await unitOfWork.SaveChangesAsync();
        
        if(customer != null)
        {
            await notificationService.SendNotificationToUserAsync(customer.Id, payload);
            if (!string.IsNullOrEmpty(customer.FcmToken))
            {
                await notificationService.SendPushNotificationAsync(
                    fcmToken: customer.FcmToken,
                    title: "Booking Cancelled",
                    body: "Technician cancelled the booking",
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
