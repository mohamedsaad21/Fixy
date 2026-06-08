using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Admin.Queries.GetDisputeById;

public sealed record GetDisputeByIdQuery(Guid DisputeId) : IRequest<Result<DisputeDetailsDto>>;
