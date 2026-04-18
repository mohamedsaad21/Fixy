using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Technicians.Queries.GetServiceRequestById;

public sealed record GetTechnicianServiceRequestByIdQuery(Guid Id) : IRequest<Result<GetTechnicianServiceRequestByIdResponse>>;
