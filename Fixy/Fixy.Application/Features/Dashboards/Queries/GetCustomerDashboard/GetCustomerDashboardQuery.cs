using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Dashboards.Queries.GetCustomerDashboard;

public sealed record GetCustomerDashboardQuery() : IRequest<Result<GetCustomerDashboardResponse>>;