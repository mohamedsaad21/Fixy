using Asp.Versioning;
using Fixy.Api.Attributes;
using Fixy.Api.Contracts.Routing;
using Fixy.Api.Controllers.Common;
using Fixy.Application.Features.Dashboards.Queries.GetAdminDashboard;
using Fixy.Application.Features.Dashboards.Queries.GetCustomerDashboard;
using Fixy.Application.Features.Dashboards.Queries.GetTechnicianDashboard;
using Fixy.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fixy.Api.Controllers.V1;

[ApiVersion("1.0")]
[Authorize]
public class DashboardsController : AppControllerBase
{
    [RedisCache(1)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet(Router.DashboardRouting.GetAdminDashboard)]
    public async Task<IActionResult> GetAdminDashboard()
    {
        return ToActionResult(await Mediator.Send(new GetAdminDashboardQuery()));
    }

    [RedisCache(1)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = Roles.Technician)]
    [HttpGet(Router.DashboardRouting.GetTechnicianDashboard)]
    public async Task<IActionResult> GetTechnicianDashboard()
    {
        return ToActionResult(await Mediator.Send(new GetTechnicianDashboardQuery()));
    }

    [RedisCache(1)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = Roles.Customer)]
    [HttpGet(Router.DashboardRouting.GetCustomerDashboard)]
    public async Task<IActionResult> GetCustomerDashboard()
    {
        return ToActionResult(await Mediator.Send(new GetCustomerDashboardQuery()));
    }
}
