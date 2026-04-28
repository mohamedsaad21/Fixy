using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Entities;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.PriceOffers.Commands.AcceptPriceOffer;

public sealed class AcceptPriceOfferCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, INotificationService notificationService) : IRequestHandler<AcceptPriceOfferCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(AcceptPriceOfferCommand request, CancellationToken cancellationToken)
    {
        var priceOffer = await unitOfWork.PriceOffers.GetTableAsTracking()
            .Include(x => x.Technician).Include(x => x.ServiceRequest).ThenInclude(x => x.Customer)
            .FirstOrDefaultAsync(x => x.Id == request.PriceOfferId);
        // check if price offer exists or not
        if (priceOffer == null)
            return Errors.PriceOfferNotFound;
        // check if the customer who create service request is the same who accept price offer
        var currentCustomer = await currentUserService.GetCurrentUserAsync();
        if (currentCustomer.Id != priceOffer.ServiceRequest.CustomerId)
            return Errors.Unauthorized;
        // check if service request not already assigned
        var serviceRequest = priceOffer.ServiceRequest;
        if (serviceRequest.Status != ServiceRequestStatus.Pending)
            return Errors.ServiceAlreadyAssigned;

        // Accept offer
        priceOffer.Status = PriceOfferStatus.Accepted;
        serviceRequest.Status = ServiceRequestStatus.Assigned;

        var booking = new ServiceBooking { ServiceRequestId = serviceRequest.Id, TechnicianId = priceOffer.TechnicianId, PriceOfferId = priceOffer.Id, AgreedPrice = priceOffer.Price, ScheduledDateTime = serviceRequest.ScheduledDateTime };
        await unitOfWork.Bookings.AddAsync(booking);
        serviceRequest.Customer.TotalBookings += 1;
        priceOffer.Technician.TotalBookings += 1;
        
        var technician = priceOffer.Technician;

        var payload = new
        {
            type = "PRICE_OFFER_ACCEPTED",
            message = $"Your price offer of {priceOffer.Price} has been accepted! A booking has been created for {booking.ScheduledDateTime:f}.",
            createdAt = DateTime.UtcNow
        };

        await notificationService.SaveNotificationAsync(technician.Id, payload.type, payload);

        await unitOfWork.SaveChangesAsync();

        await notificationService.SendNotificationToUserAsync(technician.Id, payload);

        if (!string.IsNullOrEmpty(technician.FcmToken))
        {
            await notificationService.SendPushNotificationAsync(
                fcmToken: technician.FcmToken,
                title: "Price Offer Accepted",
                body: $"Your price offer of {priceOffer.Price} has been accepted! A booking has been created for {booking.ScheduledDateTime:f}.",
                data: new Dictionary<string, string>
                {
                    { "type", "PRICE_OFFER_ACCEPTED" },
                    { "bookingId", booking.Id.ToString() },
                    { "agreedPrice", priceOffer.Price.ToString() },
                    { "scheduledDateTime", booking.ScheduledDateTime.ToString("O") },
                    { "createdAt", DateTime.UtcNow.ToString("O") }
                }
            );
        }

        return booking.Id;
    }
}
