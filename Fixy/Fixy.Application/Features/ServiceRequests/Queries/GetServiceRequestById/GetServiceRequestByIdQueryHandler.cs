using Fixy.Application.Bases;
using Fixy.Application.Mapping.ServiceRequests.Queries;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.ServiceRequests.Queries.GetServiceRequestById;

public sealed class GetServiceRequestByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetServiceRequestByIdQuery, Result<GetServiceRequestByIdDto>>
{
    public async Task<Result<GetServiceRequestByIdDto>> Handle(GetServiceRequestByIdQuery request, CancellationToken cancellationToken)
    {
        var serviceRequest = await unitOfWork.ServiceRequests.GetTableNoTracking().Include(x => x.Customer).Include(x => x.PriceOffers).ThenInclude(x => x.Technician)
            .Include(x => x.ServiceCategories)
            .FirstOrDefaultAsync(x => x.Id == request.Id);

        if (serviceRequest == null)
            return Errors.ServiceRequestNotFound;

        var serviceRequestDto = serviceRequest.ToServiceRequestByIdDto();
        return serviceRequestDto;
    }
}
