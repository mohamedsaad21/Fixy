using Fixy.Application.Abstracts;
using Fixy.Application.Bases;
using Fixy.Domain.Entities;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.PriceOffers.Commands.AcceptPriceOffer;

public class AcceptPriceOfferCommandHandler : IRequestHandler<AcceptPriceOfferCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    public AcceptPriceOfferCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(AcceptPriceOfferCommand request, CancellationToken cancellationToken)
    {
        var priceOffer = await _unitOfWork.PriceOffers.GetTableAsTracking().Include(x => x.ServiceRequest).ThenInclude(x => x.PriceOffers)
            .FirstOrDefaultAsync(x => x.Id == request.PriceOfferId);
        // check if price offer exists or not
        if (priceOffer == null)
            return Errors.PriceOfferNotFound;
        // check if the customer who create service request is the same who accept price offer
        var currentCustomer = await _currentUserService.GetCurrentUserAsync();
        if (currentCustomer.Id != priceOffer.ServiceRequest.CustomerId)
            return Errors.Unauthorized;
        // check if service request not already assigned
        var serviceRequest = priceOffer.ServiceRequest;
        if (serviceRequest.Status != ServiceRequestStatus.Pending)
            return Errors.ServiceAlreadyAssigned;
        // Accept offer
        priceOffer.Status = PriceOfferStatus.Accepted;
        serviceRequest.Status = ServiceRequestStatus.Assigned;
        // Reject other offers
        foreach(var offer in serviceRequest.PriceOffers)
        {
            if (offer.Id != priceOffer.Id)
                offer.Status = PriceOfferStatus.Rejected;
        }
        var booking = new ServiceBooking { ServiceRequestId = serviceRequest.Id, TechnicianId = priceOffer.TechnicianId, PriceOfferId = priceOffer.Id, AgreedPrice = priceOffer.Price, ScheduledDateTime = serviceRequest.ScheduledDateTime };
        await _unitOfWork.Bookings.AddAsync(booking);
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
