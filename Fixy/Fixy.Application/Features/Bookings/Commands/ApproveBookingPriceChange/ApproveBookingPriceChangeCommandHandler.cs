using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Features.Bookings.Commands.MarkBookingCompleted;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Bookings.Commands.ApproveBookingPriceChange;

public class ApproveBookingPriceChangeCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, INotificationService notificationService) : IRequestHandler<ApproveBookingPriceChangeCommand, Result>
{
    public async Task<Result> Handle(ApproveBookingPriceChangeCommand request, CancellationToken cancellationToken)
    {
        var booking = await unitOfWork.Bookings.GetTableAsTracking()
            .Include(x => x.Technician)
            .Include(x => x.ServiceRequest).ThenInclude(x => x.Customer)
            .FirstOrDefaultAsync(x => x.Id == request.BookingId);

        if (booking == null)
            return Errors.BookingNotFound;

        var currentCustomer = await currentUserService.GetCurrentUserAsync();

        if (booking.ServiceRequest.Customer.Id != currentCustomer.Id)
            return Errors.Unauthorized;

        if (booking.Status != ServiceBookingStatus.AwaitingPriceChangeApproval)
            return Errors.InvalidBookingState;

        if (booking.ProposedPrice == null)
            return Errors.NoPriceChangeToApprove;

        booking.AgreedPrice = booking.ProposedPrice.Value;
        booking.ProposedPrice = null;
        booking.Status = ServiceBookingStatus.InProgress;

        var technician = booking.Technician;

        var payload = new
        {
            type = NotificationType.PriceChangeApproved,
            message = $"The customer has approved the price change. The new agreed price is {booking.AgreedPrice}.",
            createdAt = DateTime.UtcNow
        };

        //await notificationService.SaveNotificationAsync(technician.Id, payload.type, payload);

        await unitOfWork.SaveChangesAsync();

        await notificationService.SendNotificationToUserAsync(technician.Id, payload);

        if (!string.IsNullOrEmpty(technician.FcmToken))
        {
            await notificationService.SendPushNotificationAsync(
                fcmToken: technician.FcmToken,
                title: "Price Change Approved",
                body: $"The customer has approved the price change. The new agreed price is {booking.AgreedPrice}.",
                data: new Dictionary<string, string>
                {
                    { "type", "PRICE_CHANGE_APPROVED" },
                    { "bookingId", booking.Id.ToString() },
                    { "agreedPrice", booking.AgreedPrice.ToString() },
                    { "createdAt", DateTime.UtcNow.ToString("O") }
                }
            );
        }
        return Result.Success();
    }
}
