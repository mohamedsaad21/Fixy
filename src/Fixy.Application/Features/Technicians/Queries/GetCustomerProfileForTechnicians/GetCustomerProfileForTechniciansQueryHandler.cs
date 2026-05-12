using AutoMapper;
using Fixy.Application.Bases;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Technicians.Queries.GetCustomerProfileForTechnicians;

public sealed class GetCustomerProfileForTechniciansQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetCustomerProfileForTechniciansQuery, Result<GetCustomerProfileForTechniciansResponse>>
{
    public async Task<Result<GetCustomerProfileForTechniciansResponse>> Handle(GetCustomerProfileForTechniciansQuery request, CancellationToken cancellationToken)
    {
        var customer = await unitOfWork.Customers.GetTableNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.CustomerId);

        if (customer == null)
            return Errors.CustomerNotFound;

        var response = mapper.Map<GetCustomerProfileForTechniciansResponse>(customer);
        return response;
    }
}
