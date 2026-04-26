using Fixy.Application.Bases;
using Fixy.Application.Features.Admin.Queries.GetDashboard.Responses;
using MediatR;

namespace Fixy.Application.Features.Admin.Queries.GetDashboard;

public sealed record GetDashboardQuery() : IRequest<Result<GetDashboardResponse>>;