using Fixy.Application.Bases;
using Fixy.Application.Mapping.PriceOffers.Queries;
using Fixy.Application.Mapping.ServiceRequests.Queries;
using Fixy.Application.Resources;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Features.ServiceRequests.Queries.GetServiceRequestById;

public sealed class GetCustomerServiceRequestByIdQueryHandler(IUnitOfWork unitOfWork, IStringLocalizer<SharedResources> localizer) : IRequestHandler<GetCustomerServiceRequestByIdQuery, Result<GetCustomerServiceRequestByIdResponse>>
{
    public async Task<Result<GetCustomerServiceRequestByIdResponse>> Handle(GetCustomerServiceRequestByIdQuery request, CancellationToken cancellationToken)
    {
        var serviceRequest = await unitOfWork.ServiceRequests.GetTableNoTracking()
            .Include(x => x.Customer).Include(x => x.ServiceRequestImages)
            .Include(x => x.PriceOffers).ThenInclude(x => x.Technician).ThenInclude(x => x.ServiceCategory)
            .Include(x => x.PriceOffers).ThenInclude(x => x.Technician).ThenInclude(x => x.TechnicianLocation)
            .Include(x => x.ServiceCategories)
            .FirstOrDefaultAsync(x => x.Id == request.Id);

        if (serviceRequest == null)
            return Errors.ServiceRequestNotFound;

        var serviceRequestResponse = serviceRequest.ToServiceRequestByIdResponse(localizer);
        // If service request is assigned show only accepted price offer else show all offers.
        serviceRequestResponse.PriceOffers = serviceRequest.Status == ServiceRequestStatus.Assigned ?
            serviceRequest.PriceOffers.Where(x => x.Status == PriceOfferStatus.Accepted).Select(x => x.ToPriceOfferDto(serviceRequest)).ToList()
            :
            serviceRequest.PriceOffers
            .Select(x => x.ToPriceOfferDto(serviceRequest)).OrderByDescending(x => x.AverageRating).ThenBy(x => x.DistanceKm)
            .ThenBy(x => x.Price).ToList();
        return serviceRequestResponse;
    }
}
