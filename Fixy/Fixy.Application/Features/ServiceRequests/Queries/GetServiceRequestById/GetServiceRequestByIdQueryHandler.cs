using Fixy.Application.Bases;
using Fixy.Application.Mapping.ServiceRequests.Queries;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.ServiceRequests.Queries.GetServiceRequestById;

public sealed class GetServiceRequestByIdQueryHandler : IRequestHandler<GetServiceRequestByIdQuery, Result<GetServiceRequestByIdDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetServiceRequestByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<GetServiceRequestByIdDto>> Handle(GetServiceRequestByIdQuery request, CancellationToken cancellationToken)
    {
        var serviceRequest = await _unitOfWork.ServiceRequests.GetTableNoTracking().Include(x => x.Customer).Include(x => x.PriceOffers).ThenInclude(x => x.Technician)
            .Include(x => x.ServiceCategories)
            .FirstOrDefaultAsync(x => x.Id == request.Id);
        if (serviceRequest == null)
            return Errors.ServiceRequestNotFound;
        var serviceRequestDto = serviceRequest.ToServiceRequestByIdDto();
        return serviceRequestDto;
    }
}
