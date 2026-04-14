using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Entities;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.PriceOffers.Commands.AcceptPriceOffer;

public sealed class AcceptPriceOfferCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService) : IRequestHandler<AcceptPriceOfferCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(AcceptPriceOfferCommand request, CancellationToken cancellationToken)
    {
        var priceOffer = await unitOfWork.PriceOffers.GetTableAsTracking().Include(x => x.ServiceRequest).ThenInclude(x => x.PriceOffers)
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
        serviceRequest.Status = ServiceRequestStatus.Accepted;

        var booking = new ServiceBooking { ServiceRequestId = serviceRequest.Id, TechnicianId = priceOffer.TechnicianId, PriceOfferId = priceOffer.Id, AgreedPrice = priceOffer.Price, ScheduledDateTime = serviceRequest.ScheduledDateTime };
        await unitOfWork.Bookings.AddAsync(booking);
        await unitOfWork.SaveChangesAsync();
        
        return booking.Id;
    }
}
