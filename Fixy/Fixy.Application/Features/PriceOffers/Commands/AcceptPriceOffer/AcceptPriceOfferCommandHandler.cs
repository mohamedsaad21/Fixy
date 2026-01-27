using Fixy.Application.Abstracts;
using Fixy.Application.Bases;
using Fixy.Domain.Enums;
using Fixy.Infrastructure.Persistence.Abstracts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.PriceOffers.Commands.AcceptPriceOffer;

public class AcceptPriceOfferCommandHandler : IRequestHandler<AcceptPriceOfferCommand, Result>
{
    private readonly IPriceOfferRepository _priceOfferRepository;
    private readonly ICurrentUserService _currentUserService;
    public AcceptPriceOfferCommandHandler(IPriceOfferRepository priceOfferRepository, ICurrentUserService currentUserService)
    {
        _priceOfferRepository = priceOfferRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(AcceptPriceOfferCommand request, CancellationToken cancellationToken)
    {
        var priceOffer = await _priceOfferRepository.GetTableAsTracking().Include(x => x.ServiceRequest).ThenInclude(x => x.PriceOffers)
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
        await _priceOfferRepository.SaveChangesAsync();
        return Result.Success();
    }
}
