using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Bookings.Commands.RequestBookingPriceChange;

public class RequestBookingPriceChangeCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, INotificationService notificationService) : IRequestHandler<RequestBookingPriceChangeCommand, Result>
{
    public async Task<Result> Handle(RequestBookingPriceChangeCommand request, CancellationToken cancellationToken)
    {
        var booking = await unitOfWork.Bookings.GetTableAsTracking()
            .Include(x => x.ServiceRequest).ThenInclude(x => x.Customer)
            .FirstOrDefaultAsync(x => x.Id == request.BookingId);
        // check if booking exists or not
        if (booking == null)
            return Errors.BookingNotFound;

        var currentTechnician = await currentUserService.GetCurrentUserAsync();
        // Ensure that technician who involved in booking is same who request price change
        if (booking.TechnicianId != currentTechnician.Id)
            return Errors.Unauthorized;

        if (booking.Status != ServiceBookingStatus.InProgress)
            return Errors.BookingNotActive;

        if (booking.ProposedPrice.HasValue)
            return Errors.PriceChangeAlreadyPending;

        if (request.NewProposedPrice == booking.AgreedPrice)
            return Errors.AlreadyAgreedPrice;

        booking.ProposedPrice = request.NewProposedPrice;
        booking.PriceChangeRequestedAt = DateTime.UtcNow;
        booking.PriceChangeNotes = request.Notes;
        booking.Status = ServiceBookingStatus.AwaitingPriceChangeApproval;

        await unitOfWork.SaveChangesAsync();

        var customer = booking.ServiceRequest.Customer;
        await notificationService.SendNotificationToUserAsync(customer.Id, new
        {
            type = "PRICE_CHANGE_REQUESTED",
            message = $"Your technician has requested a price change to {request.NewProposedPrice}. Please review and approve or reject the new price.",
            createdAt = DateTime.UtcNow
        });

        if (!string.IsNullOrEmpty(customer.FcmToken))
        {
            await notificationService.SendPushNotificationAsync(
                fcmToken: customer.FcmToken,
                title: "Price Change Requested",
                body: $"Your technician has requested a price change to {request.NewProposedPrice}. Please review and approve or reject the new price.",
                data: new Dictionary<string, string>
                {
                    { "type", "PRICE_CHANGE_REQUESTED" },
                    { "bookingId", booking.Id.ToString() },
                    { "newProposedPrice", request.NewProposedPrice.ToString() },
                    { "createdAt", DateTime.UtcNow.ToString("O") }
                }
            );
        }
        return Result.Success();
    }
}
