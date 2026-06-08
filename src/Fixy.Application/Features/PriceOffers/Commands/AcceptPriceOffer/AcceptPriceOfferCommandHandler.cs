using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Resources;
using Fixy.Domain.Entities;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Fixy.Application.Features.PriceOffers.Commands.AcceptPriceOffer;

public sealed class AcceptPriceOfferCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService,
    INotificationService notificationService, IStringLocalizer<SharedResources> localizer, ILogger<AcceptPriceOfferCommandHandler> logger) : IRequestHandler<AcceptPriceOfferCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(AcceptPriceOfferCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Customer attempting to accept price offer. PriceOfferId: {PriceOfferId}", request.PriceOfferId);
            
        var priceOffer = await unitOfWork.PriceOffers.GetTableAsTracking()
            .Include(x => x.Technician).Include(x => x.ServiceRequest).ThenInclude(x => x.Customer)
            .FirstOrDefaultAsync(x => x.Id == request.PriceOfferId);
        // check if price offer exists or not
        if (priceOffer == null)
        {
            logger.LogWarning("Price offer acceptance failed — offer not found. PriceOfferId: {PriceOfferId}", request.PriceOfferId);
            return Errors.PriceOfferNotFound;
        }
        // check if the customer who create service request is the same who accept price offer
        var currentCustomer = await currentUserService.GetCurrentUserAsync();
        if (currentCustomer.Id != priceOffer.ServiceRequest.CustomerId)
        {
            logger.LogWarning("Price offer acceptance failed — unauthorized customer. PriceOfferId: {PriceOfferId}, ServiceRequestCustomerId: {ServiceRequestCustomerId}, RequestingCustomerId: {RequestingCustomerId}", request.PriceOfferId, priceOffer.ServiceRequest.CustomerId, currentCustomer.Id);
            return Errors.Unauthorized;
        }
        // check if service request not already assigned
        var serviceRequest = priceOffer.ServiceRequest;
        if (serviceRequest.Status != ServiceRequestStatus.Pending)
        {
            logger.LogWarning("Price offer acceptance failed — service request already assigned. PriceOfferId: {PriceOfferId}, ServiceRequestId: {ServiceRequestId}, CurrentStatus: {CurrentStatus}", request.PriceOfferId, serviceRequest.Id, serviceRequest.Status);
            return Errors.ServiceAlreadyAssigned;
        }

        // Accept offer
        priceOffer.Status = PriceOfferStatus.Accepted;
        serviceRequest.Status = ServiceRequestStatus.Assigned;

        var booking = new ServiceBooking { ServiceRequestId = serviceRequest.Id, TechnicianId = priceOffer.TechnicianId, PriceOfferId = priceOffer.Id, AgreedPrice = priceOffer.Price, ScheduledDateTime = serviceRequest.ScheduledDateTime };
        await unitOfWork.Bookings.AddAsync(booking);
        serviceRequest.Customer.TotalBookings += 1;
        priceOffer.Technician.TotalBookings += 1;
        
        var technician = priceOffer.Technician;

        await notificationService.SendFullNotificationAsync(
            technician,
            NotificationType.PriceOfferAccepted,
            SharedResourcesKeys.NotificationPriceOfferAcceptedTitle,
            SharedResourcesKeys.NotificationPriceOfferAcceptedBody
        );
        await unitOfWork.SaveChangesAsync();
        logger.LogInformation("Price offer accepted — booking created successfully. PriceOfferId: {PriceOfferId}, BookingId: {BookingId}, ServiceRequestId: {ServiceRequestId}, CustomerId: {CustomerId}, TechnicianId: {TechnicianId}, AgreedPrice: {AgreedPrice}, ScheduledDateTime: {ScheduledDateTime}",
            priceOffer.Id, booking.Id, serviceRequest.Id, currentCustomer.Id,
            priceOffer.TechnicianId, booking.AgreedPrice, booking.ScheduledDateTime);
        return booking.Id;
    }
}
