using AutoMapper;
using Fixy.Application.Bases;
using Fixy.Application.Features.ServiceRequests.Queries.Models;
using Fixy.Application.Features.ServiceRequests.Queries.Results;
using Fixy.Infrastructure.Persistence.Abstracts;
using MediatR;

namespace Fixy.Application.Features.ServiceRequests.Queries.Handlers;

public class ServiceRequestsQueriesHandler : IRequestHandler<GetRequestsListQuery, Result<List<GetRequestsListResponse>>>
{
    private readonly IServiceRequestReadRepository _serviceRequestReadRepository;
    private readonly IMapper _mapper;

    public ServiceRequestsQueriesHandler(IServiceRequestReadRepository serviceRequestReadRepository, IMapper mapper)
    {
        _serviceRequestReadRepository = serviceRequestReadRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<GetRequestsListResponse>>> Handle(GetRequestsListQuery request, CancellationToken cancellationToken)
    {
        var serviceRequests = await _serviceRequestReadRepository.GetServiceRequestsAsync();
        var serviceRequestsMapper = _mapper.Map<List<GetRequestsListResponse>>(serviceRequests);
        return serviceRequestsMapper;
    }
}
