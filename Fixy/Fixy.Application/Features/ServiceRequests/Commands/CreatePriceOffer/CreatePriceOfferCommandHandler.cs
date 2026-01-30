using Fixy.Application.Abstracts;
using Fixy.Application.Bases;
using Fixy.Application.Mapping.PriceOffers.Commands;
using Fixy.Domain.Entities;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.ServiceRequests.Commands.CreatePriceOffer;

public class CreatePriceOfferCommandHandler : IRequestHandler<CreatePriceOfferCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    public CreatePriceOfferCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(CreatePriceOfferCommand request, CancellationToken cancellationToken)
    {
        var currentTechnician = await _currentUserService.GetCurrentUserAsync();
        if (currentTechnician is not Technician technician)
            return Errors.Unauthorized;

        var serviceRequest = await _unitOfWork.ServiceRequests.GetTableAsTracking().Include(x => x.PriceOffers).FirstOrDefaultAsync(x => x.Id == request.ServiceRequestId);
        if (serviceRequest == null)
            return Errors.ServiceRequestNotFound;

        if (serviceRequest.PriceOffers.Any(x => x.TechnicianId == technician.Id))
            return Errors.AlreadyCreatedPriceOffer;

        var priceOffer = request.ToPriceOfferDomain();
        priceOffer.TechnicianId = technician.Id;

        serviceRequest.PriceOffers.Add(priceOffer);
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
