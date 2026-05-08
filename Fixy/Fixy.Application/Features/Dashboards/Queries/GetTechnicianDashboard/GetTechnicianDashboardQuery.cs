using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Dashboards.Queries.GetTechnicianDashboard;

public sealed record GetTechnicianDashboardQuery() : IRequest<Result<GetTechnicianDashboardResponse>>;