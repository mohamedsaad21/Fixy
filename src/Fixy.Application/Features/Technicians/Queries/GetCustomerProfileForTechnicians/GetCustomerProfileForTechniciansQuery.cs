using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Technicians.Queries.GetCustomerProfileForTechnicians;

public sealed record GetCustomerProfileForTechniciansQuery
    (
        Guid CustomerId
    ) : IRequest<Result<GetCustomerProfileForTechniciansResponse>>;