using Fixy.Application.Bases;
using Fixy.Application.Mapping.ServiceRequests.Queries;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.ServiceRequests.Queries.GetServiceRequestById;

public sealed class GetCustomerServiceRequestByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetCustomerServiceRequestByIdQuery, Result<GetCustomerServiceRequestByIdResponse>>
{
    public async Task<Result<GetCustomerServiceRequestByIdResponse>> Handle(GetCustomerServiceRequestByIdQuery request, CancellationToken cancellationToken)
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
