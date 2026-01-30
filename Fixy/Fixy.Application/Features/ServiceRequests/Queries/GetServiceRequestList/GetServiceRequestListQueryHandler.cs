using AutoMapper;
using Fixy.Application.Bases;
using Fixy.Application.Mapping.ServiceRequests;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.ServiceRequests.Queries.GetServiceRequestList;

public class GetServiceRequestListQueryHandler : IRequestHandler<GetServiceRequestListQuery, Result<List<GetServiceRequestListDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetServiceRequestListQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<List<GetServiceRequestListDto>>> Handle(GetServiceRequestListQuery request, CancellationToken cancellationToken)
    {
        var serviceRequests = await _unitOfWork.ServiceRequests.GetTableNoTracking().Include(x => x.Customer).Include(x => x.ServiceCategories).ToListAsync();
        var result = serviceRequests.Select(x => x.ToServiceRequestListDto()).ToList();
        return result;
    }
}
