using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Resources;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fixy.Application.Features.Bookings.Commands.RequestBookingPriceChange;

public class RequestBookingPriceChangeCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, INotificationService notificationService, ILogger<RequestBookingPriceChangeCommandHandler>logger) : IRequestHandler<RequestBookingPriceChangeCommand, Result>
{
    public async Task<Result> Handle(RequestBookingPriceChangeCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Technician attempting to request price change. BookingId: {BookingId}, NewProposedPrice: {NewProposedPrice}", request.BookingId, request.NewProposedPrice);

        var booking = await unitOfWork.Bookings.GetTableAsTracking()
            .Include(x => x.ServiceRequest).ThenInclude(x => x.Customer)
            .FirstOrDefaultAsync(x => x.Id == request.BookingId);
        // check if booking exists or not
        if (booking == null)
        {
            logger.LogWarning("Price change request failed — booking not found. BookingId: {BookingId}", request.BookingId);
            return Errors.BookingNotFound;
        }

        var currentTechnician = await currentUserService.GetCurrentUserAsync();
        // Ensure that technician who involved in booking is same who request price change
        if (booking.TechnicianId != currentTechnician.Id)
        {
            logger.LogWarning("Price change request failed — unauthorized technician. BookingId: {BookingId}, BookingTechnicianId: {BookingTechnicianId}, RequestingTechnicianId: {RequestingTechnicianId}", request.BookingId, booking.TechnicianId, currentTechnician.Id);
            return Errors.Unauthorized;
        }

        if (booking.Status != ServiceBookingStatus.InProgress)
        {
            logger.LogWarning("Price change request failed — booking is not active. BookingId: {BookingId}, CurrentStatus: {CurrentStatus}", request.BookingId, booking.Status);
            return Errors.BookingNotActive;
        }

        if (booking.ProposedPrice.HasValue)
        {
            logger.LogWarning("Price change request failed — a price change is already pending. BookingId: {BookingId}, ExistingProposedPrice: {ExistingProposedPrice}", request.BookingId, booking.ProposedPrice);
            return Errors.PriceChangeAlreadyPending;
        }

        if (request.NewProposedPrice == booking.AgreedPrice)
        {
            logger.LogWarning("Price change request failed — proposed price matches current agreed price. BookingId: {BookingId}, AgreedPrice: {AgreedPrice}, ProposedPrice: {ProposedPrice}", request.BookingId, booking.AgreedPrice, request.NewProposedPrice);
            return Errors.AlreadyAgreedPrice;
        }

        if (booking.HasRequestedPriceChange)
        {
            logger.LogWarning("Price change request failed — one-time price change limit already used. BookingId: {BookingId}, TechnicianId: {TechnicianId}", request.BookingId, currentTechnician.Id);
            return Errors.PriceChangeAlreadyRequested;
        }

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
        logger.LogInformation("Price change requested successfully. BookingId: {BookingId}, TechnicianId: {TechnicianId}, CustomerId: {CustomerId}, OriginalPrice: {OriginalPrice}, ProposedPrice: {ProposedPrice}",
            request.BookingId, currentTechnician.Id, booking.ServiceRequest.Customer.Id,
            booking.AgreedPrice, request.NewProposedPrice);
        return Result.Success();
    }
}