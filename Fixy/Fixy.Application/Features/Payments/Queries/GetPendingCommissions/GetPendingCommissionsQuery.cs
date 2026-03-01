using Fixy.Application.Bases;
using Fixy.Application.Features.Payments.Queries.GetPendingCommissions.Responses;
using MediatR;

namespace Fixy.Application.Features.Payments.Queries.GetPendingCommissions;

public record GetPendingCommissionsQuery() : IRequest<Result<GetPendingCommissionsResponse>>;
