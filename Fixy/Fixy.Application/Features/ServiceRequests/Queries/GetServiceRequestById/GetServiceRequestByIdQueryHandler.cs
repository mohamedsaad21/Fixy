using Fixy.Application.Bases;
using Fixy.Application.Mapping.ServiceRequests.Queries;
using Fixy.Infrastructure.Persistence.Abstracts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.ServiceRequests.Queries.GetServiceRequestById;

public class GetServiceRequestByIdQueryHandler : IRequestHandler<GetServiceRequestByIdQuery, Result<GetServiceRequestByIdDto>>
{
    private readonly IServiceRequestRepository _serviceRequestRepository;

    public GetServiceRequestByIdQueryHandler(IServiceRequestRepository serviceRequestRepository)
    {
        _serviceRequestRepository = serviceRequestRepository;
    }

    public async Task<Result<GetServiceRequestByIdDto>> Handle(GetServiceRequestByIdQuery request, CancellationToken cancellationToken)
    {
        var serviceRequest = await _serviceRequestRepository.GetTableNoTracking().Include(x => x.Customer).Include(x => x.PriceOffers).ThenInclude(x => x.Technician)
            .Include(x => x.ServiceCategories)
            .FirstOrDefaultAsync(x => x.Id == request.Id);
        if (serviceRequest == null)
            return Errors.ServiceRequestNotFound;
        var serviceRequestDto = serviceRequest.ToServiceRequestByIdDto();
        return serviceRequestDto;
    }
}
