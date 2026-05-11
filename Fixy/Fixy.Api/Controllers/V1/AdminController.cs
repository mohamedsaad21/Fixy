using Asp.Versioning;
using Fixy.Api.Attributes;
using Fixy.Api.Contracts.Routing;
using Fixy.Api.Controllers.Common;
using Fixy.Application.Features.Admin.Commands.ApproveTechnician;
using Fixy.Application.Features.Admin.Commands.BlockCustomer;
using Fixy.Application.Features.Admin.Commands.BlockTecnhnician;
using Fixy.Application.Features.Admin.Commands.RejectTechnician;
using Fixy.Application.Features.Admin.Commands.UnblockCustomer;
using Fixy.Application.Features.Admin.Commands.UnblockTechnician;
using Fixy.Application.Features.Admin.Queries.GetBookingById;
using Fixy.Application.Features.Admin.Queries.GetBookings;
using Fixy.Application.Features.Admin.Queries.GetCustomers;
using Fixy.Application.Features.Admin.Queries.GetTechnicians;
using Fixy.Application.Features.Admin.Queries.GetUserInfoById;
using Fixy.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fixy.Api.Controllers.V1;

[ApiVersion("1.0")]
[Authorize(Roles = Roles.Admin)]
public class AdminController : AppControllerBase
{
    //[RedisCache(3)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet(Router.AdminRouting.GetTechnicians)]
    public async Task<IActionResult> GetTechnicians([FromQuery] GetTechniciansQuery query)
    {
        return ToActionResult(await Mediator.Send(query));
    }

    //[RedisCache(3)]
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
    [HttpPut(Router.AdminRouting.ApproveTechnician)]
    public async Task<IActionResult> ApproveTechnician([FromRoute] Guid TechnicianId)
    {
        return ToActionResult(await Mediator.Send(new ApproveTechnicianCommand(TechnicianId)));
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPut(Router.AdminRouting.RejectTechnician)]
    public async Task<IActionResult> RejectTechnician([FromForm] RejectTechnicianCommand command)
    {
        return ToActionResult(await Mediator.Send(command));
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPut(Router.AdminRouting.BlockTechnician)]
    public async Task<IActionResult> BlockTechnician([FromForm] BlockTecnhnicianCommand command)
    {
        return ToActionResult(await Mediator.Send(command));
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPut(Router.AdminRouting.BlockCustomer)]
    public async Task<IActionResult> BlockCustomer([FromForm] BlockCustomerCommand command)
    {
        return ToActionResult(await Mediator.Send(command));
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPut(Router.AdminRouting.UnblockCustomer)]
    public async Task<IActionResult> UnblockCustomer([FromRoute] Guid CustomerId)
    {
        return ToActionResult(await Mediator.Send(new UnblockCustomerCommand(CustomerId)));
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPut(Router.AdminRouting.UnblockTechnician)]
    public async Task<IActionResult> UnblockTechnician([FromRoute] Guid TechnicianId)
    {
        return ToActionResult(await Mediator.Send(new UnblockTechnicianCommand(TechnicianId)));
    }

    //[RedisCache(3)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet(Router.AdminRouting.GetBookings)]
    public async Task<IActionResult> GetBookings([FromQuery] GetBookingsQuery query)
    {
        return ToActionResult(await Mediator.Send(query));
    }

    //[RedisCache(3)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet(Router.AdminRouting.GetBookingById)]
    public async Task<IActionResult> GetBookingsById([FromRoute] Guid Id)
    {
        return ToActionResult(await Mediator.Send(new GetBookingByIdQuery(Id)));
    }

    //[RedisCache(3)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet(Router.AdminRouting.GetUserInfoById)]
    public async Task<IActionResult> GetUserInfoById([FromRoute] Guid UserId)
    {
        return ToActionResult(await Mediator.Send(new GetUserInfoByIdQuery(UserId)));
    }
}
