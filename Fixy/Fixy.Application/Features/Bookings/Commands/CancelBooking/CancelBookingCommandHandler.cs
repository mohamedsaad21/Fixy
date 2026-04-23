using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Entities.Identity;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Bookings.Commands.CancelBooking;

public sealed class CancelBookingCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, INotificationService notificationService, UserManager<ApplicationUser> userManager) : IRequestHandler<CancelBookingCommand, Result>
{
    public async Task<Result> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
    {
        var user = await currentUserService.GetCurrentUserAsync();

        if (user == null)
            return Errors.Unauthorized;

        var booking = await unitOfWork.Bookings.GetTableAsTracking().Include(x => x.ServiceRequest)
            .FirstOrDefaultAsync(x => x.Id == request.BookingId);

        if (booking == null)
            return Errors.BookingNotFound;

        if (booking.ServiceRequest.CustomerId != user.Id && booking.TechnicianId != user.Id)
            return Errors.Unauthorized;

        if (booking.Status == ServiceBookingStatus.Cancelled)
            return Errors.AlreadyCancelled;

        if(booking.Status != ServiceBookingStatus.InProgress && booking.Status != ServiceBookingStatus.PriceChangePendingCustomerApproval)
            return Errors.CannotCancelAtThisStage;

        booking.Status = ServiceBookingStatus.Cancelled;
        booking.CancelledAt = DateTime.UtcNow;
        booking.CancelledById = user.Id;
        await unitOfWork.SaveChangesAsync();
        // send notification to other user
        var otherUserId = booking.ServiceRequest.CustomerId == user.Id ? booking.TechnicianId : booking.ServiceRequest.CustomerId;
        var otherUser = await userManager.Users.FirstOrDefaultAsync(x => x.Id == otherUserId);
        
        if(otherUser != null)
        {
            await notificationService.SendNotificationToUserAsync(otherUser.Id, new
            {
                type = "BOOKING_CANCELLED",
                Message = $"Booking has been cancelled",
                CreatedAt = DateTime.UtcNow
            });
            if (!string.IsNullOrEmpty(otherUser.FcmToken))
            {
                await notificationService.SendPushNotificationAsync(
                    fcmToken: otherUser.FcmToken,
                    title: "Booking Cancelled",
                    body: "Your booking has been Cancelled",
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
