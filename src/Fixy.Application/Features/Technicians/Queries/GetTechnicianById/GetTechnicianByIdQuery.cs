using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Technicians.Queries.GetTechnicianById;

public sealed record GetTechnicianByIdQuery(Guid Id) : IRequest<Result<GetTechnicianByIdResponse>>;
