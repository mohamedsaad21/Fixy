using AutoMapper;
using Fixy.Application.Bases;
using Fixy.Application.Mapping.ServiceRequests;
using Fixy.Infrastructure.Persistence.Abstracts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.ServiceRequests.Queries.GetServiceRequestList;

public class GetServiceRequestListQueryHandler : IRequestHandler<GetServiceRequestListQuery, Result<List<GetServiceRequestListDto>>>
{
    private readonly IServiceRequestRepository _serviceRequestRepository;
    private readonly IMapper _mapper;

    public GetServiceRequestListQueryHandler(IServiceRequestRepository serviceRequestRepository, IMapper mapper)
    {
        _serviceRequestRepository = serviceRequestRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<GetServiceRequestListDto>>> Handle(GetServiceRequestListQuery request, CancellationToken cancellationToken)
    {
        var serviceRequests = await _serviceRequestRepository.GetTableNoTracking().Include(x => x.Customer).Include(x => x.ServiceCategories).ToListAsync();
        var result = serviceRequests.Select(x => x.ToServiceRequestListDto()).ToList();
        return result;
    }
}
