using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Bookings.Commands.RejectBookingPriceChange;

public class RejectBookingPriceChangeCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, INotificationService notificationService) : IRequestHandler<RejectBookingPriceChangeCommand, Result>
{
    public async Task<Result> Handle(RejectBookingPriceChangeCommand request, CancellationToken cancellationToken)
    {
        var booking = await unitOfWork.Bookings.GetTableAsTracking()
            .Include(x => x.Technician).Include(x => x.ServiceRequest).ThenInclude(x => x.Customer)
            .FirstOrDefaultAsync(x => x.Id == request.BookingId);

        if (booking == null)
            return Errors.BookingNotFound;

        var currentCustomer = await currentUserService.GetCurrentUserAsync();

        if (booking.ServiceRequest.Customer.Id != currentCustomer.Id)
            return Errors.Unauthorized;

        if (booking.Status != ServiceBookingStatus.AwaitingPriceChangeApproval)
            return Errors.InvalidBookingState;

        booking.ProposedPrice = null;
        booking.Status = ServiceBookingStatus.InProgress;

        var technician = booking.Technician;

        var payload = new
        {
            type = NotificationType.PriceChangeRejected,
            message = $"The customer has rejected the price change. The original agreed price of {booking.AgreedPrice} remains.",
            createdAt = DateTime.UtcNow
        };

        //await notificationService.SaveNotificationAsync(technician.Id, payload.type, payload);

        await unitOfWork.SaveChangesAsync();

        await notificationService.SendNotificationToUserAsync(technician.Id, payload);

        if (!string.IsNullOrEmpty(technician.FcmToken))
        {
            await notificationService.SendPushNotificationAsync(
                fcmToken: technician.FcmToken,
                title: "Price Change Rejected",
                body: $"The customer has rejected the price change. The original agreed price of {booking.AgreedPrice} remains.",
                data: new Dictionary<string, string>
                {
                    { "type", "PRICE_CHANGE_REJECTED" },
                    { "bookingId", booking.Id.ToString() },
                    { "agreedPrice", booking.AgreedPrice.ToString() },
                    { "createdAt", DateTime.UtcNow.ToString("O") }
                }
            );
        }
        return Result.Success();
    }
}
