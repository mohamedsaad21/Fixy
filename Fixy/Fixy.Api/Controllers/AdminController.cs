using Fixy.Api.Base;
using Fixy.Api.Contracts.Routing;
using Fixy.Application.Features.Admin.Commands.ApproveTechnician;
using Fixy.Application.Features.Admin.Commands.BlockCustomer;
using Fixy.Application.Features.Admin.Commands.BlockTecnhnician;
using Fixy.Application.Features.Admin.Commands.RejectTechnician;
using Fixy.Application.Features.Admin.Queries.GetCustomers;
using Fixy.Application.Features.Admin.Queries.GetDashboard;
using Fixy.Application.Features.Admin.Queries.GetTechnicians;
using Fixy.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fixy.Api.Controllers;

[Authorize(Roles = Roles.Admin)]
public class AdminController : AppControllerBase
{
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet(Router.AdminRouting.GetDashboard)]
    public async Task<IActionResult> GetDashboard()
    {
        return ToActionResult(await Mediator.Send(new GetDashboardQuery()));
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet(Router.AdminRouting.GetTechnicians)]
    public async Task<IActionResult> GetTechnicians([FromQuery] GetTechniciansQuery query)
    {
        return ToActionResult(await Mediator.Send(query));
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet(Router.AdminRouting.GetCustomers)]
    public async Task<IActionResult> GetCustomers([FromQuery] GetCustomersQuery query)
    {
        return ToActionResult(await Mediator.Send(query));
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPost(Router.AdminRouting.ApproveTechnician)]
    public async Task<IActionResult> ApproveTechnician([FromRoute] Guid TechnicianId)
    {
        return ToActionResult(await Mediator.Send(new ApproveTechnicianCommand(TechnicianId)));
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPost(Router.AdminRouting.RejectTechnician)]
    public async Task<IActionResult> RejectTechnician([FromForm] RejectTechnicianCommand command)
    {
        return ToActionResult(await Mediator.Send(command));
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPost(Router.AdminRouting.BlockTechnician)]
    public async Task<IActionResult> BlockTechnician([FromForm] BlockTecnhnicianCommand command)
    {
        return ToActionResult(await Mediator.Send(command));
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPost(Router.AdminRouting.BlockCustomer)]
    public async Task<IActionResult> BlockCustomer([FromForm] BlockCustomerCommand command)
    {
        return ToActionResult(await Mediator.Send(command));
    }
}
