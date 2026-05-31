using Fixy.Application.Bases;
using Fixy.Domain.SP.TechnicianCashCommissionsOwed.Responses;
using MediatR;

namespace Fixy.Application.Features.Payments.Queries.GetTechnicianCashCommissionsOwed;

public sealed record GetTechnicianCashCommissionsOwedQuery() : IRequest<Result<GetTechnicianCashCommissionsOwedResponse>>;
