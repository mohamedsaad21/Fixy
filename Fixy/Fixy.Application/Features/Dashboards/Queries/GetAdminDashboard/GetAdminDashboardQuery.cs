using Fixy.Application.Bases;
using Fixy.Application.Features.Dashboards.Queries.GetAdminDashboard.Responses;
using MediatR;

namespace Fixy.Application.Features.Dashboards.Queries.GetAdminDashboard;

public sealed record GetAdminDashboardQuery() : IRequest<Result<GetDashboardResponse>>;