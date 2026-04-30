using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Resources;
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

        if (booking.HasRequestedPriceChange)
            return Errors.PriceChangeAlreadyRequested;

        booking.ProposedPrice = request.NewProposedPrice;
        booking.PriceChangeRequestedAt = DateTime.UtcNow;
        booking.PriceChangeNotes = request.Notes;
        booking.Status = ServiceBookingStatus.AwaitingPriceChangeApproval;
        booking.HasRequestedPriceChange = true;

        var customer = booking.ServiceRequest.Customer;

        await notificationService.SendFullNotificationAsync(
            customer,
            NotificationType.PriceChangeRequested,
            SharedResourcesKeys.NotificationPriceChangeRequestedTitle,
            SharedResourcesKeys.NotificationPriceChangeRequestedBody
        );
        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
