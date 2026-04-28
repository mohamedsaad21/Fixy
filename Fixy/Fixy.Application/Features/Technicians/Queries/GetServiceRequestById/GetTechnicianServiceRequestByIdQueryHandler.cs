using AutoMapper;
using Fixy.Application.Bases;
using Fixy.Application.Common.Helpers;
using Fixy.Application.Resources;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Features.Technicians.Queries.GetServiceRequestById;

public sealed class GetTechnicianServiceRequestByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IStringLocalizer<SharedResources> localizer) : IRequestHandler<GetTechnicianServiceRequestByIdQuery, Result<GetTechnicianServiceRequestByIdResponse>>
{
    public async Task<Result<GetTechnicianServiceRequestByIdResponse>> Handle(GetTechnicianServiceRequestByIdQuery request, CancellationToken cancellationToken)
    {
        var serviceRequest = await unitOfWork.ServiceRequests.GetTableNoTracking().Include(x => x.Customer).Include(x => x.PriceOffers).ThenInclude(x => x.Technician)
            .Include(x => x.ServiceCategories)
            .FirstOrDefaultAsync(x => x.Id == request.Id);

        if (serviceRequest == null)
            return Errors.ServiceRequestNotFound;

        var serviceRequestResponse = mapper.Map<GetTechnicianServiceRequestByIdResponse>(serviceRequest);
        serviceRequestResponse.Status = EnumLocalizer.Localize(serviceRequest.Status, localizer);
        return serviceRequestResponse;
    }
}
