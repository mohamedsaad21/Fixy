using Fixy.Application.Abstracts;
using Fixy.Application.Bases;
using Fixy.Application.Mapping.PriceOffers.Commands;
using Fixy.Domain.Entities;
using Fixy.Infrastructure.Persistence.Abstracts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.ServiceRequests.Commands.CreatePriceOffer;

public class CreatePriceOfferCommandHandler : IRequestHandler<CreatePriceOfferCommand, Result>
{
    private readonly IServiceRequestRepository _serviceRequestRepository;
    private readonly ICurrentUserService _currentUserService;
    public CreatePriceOfferCommandHandler(IServiceRequestRepository serviceRequestRepository, ICurrentUserService currentUserService)
    {
        _serviceRequestRepository = serviceRequestRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(CreatePriceOfferCommand request, CancellationToken cancellationToken)
    {
        var currentTechnician = await _currentUserService.GetCurrentUserAsync();
        if (currentTechnician is not Technician technician)
            return Errors.Unauthorized;

        var serviceRequest = await _serviceRequestRepository.GetTableNoTracking().Include(x => x.PriceOffers).FirstOrDefaultAsync(x => x.Id == request.ServiceRequestId);
        if (serviceRequest == null)
            return Errors.ServiceRequestNotFound;

        if (serviceRequest.PriceOffers.Any(x => x.TechnicianId == technician.Id))
            return Errors.AlreadyCreatedPriceOffer;

        var priceOffer = request.ToPriceOfferDomain();
        priceOffer.TechnicianId = technician.Id;

        serviceRequest.PriceOffers.Add(priceOffer);
        await _serviceRequestRepository.UpdateAsync(serviceRequest);
        return Result.Success();
    }
}
