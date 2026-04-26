using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Technicians.Queries.GetTechnicianProfileForCustomers;

public sealed record GetTechnicianProfileForCustomersQuery
    (
        Guid TechnicianId
    ) : IRequest<Result<GetTechnicianProfileForCustomersResponse>>;