using Fixy.Api.Base;
using Fixy.Api.Contracts.Routing;
using Fixy.Application.Features.Technicians.Commands.UpdateTechnicianLocation;
using Fixy.Application.Features.Technicians.Commands.UpdateTechnicianProfile;
using Fixy.Application.Features.Technicians.Queries.GetServiceRequestById;
using Fixy.Application.Features.Technicians.Queries.GetTechnicianAvailableRequests;
using Fixy.Application.Features.Technicians.Queries.GetTechnicianById;
using Fixy.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fixy.Api.Controllers;

[Authorize]
public class TechnicianController : AppControllerBase
{
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet(Router.TechnicianRouting.GetById)]
    public async Task<IActionResult> GetTechnicianById([FromRoute] Guid Id)
    {
        return ToActionResult(await Mediator.Send(new GetTechnicianByIdQuery(Id)));
    }

    [Authorize(Roles = Roles.Technician)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet(Router.TechnicianRouting.TechnicianServiceRequestsList)]
    public async Task<IActionResult> GetTechnicianAvailableServiceRequests([FromQuery] GetTechnicianAvailableRequestsQuery query)
    {
        return ToActionResult(await Mediator.Send(query));
    }

    [Authorize(Roles = Roles.Technician)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet(Router.TechnicianRouting.ServiceRequestById)]
    public async Task<IActionResult> GetServiceRequestById([FromRoute] GetTechnicianServiceRequestByIdQuery query)
    {
        return ToActionResult(await Mediator.Send(query));
    }

    [Authorize(Roles = Roles.Technician)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPost(Router.TechnicianRouting.Location)]
    public async Task<IActionResult> UpdateTechnicianLocation([FromForm] UpdateTechnicianLocationCommand command)
    {
        return ToActionResult(await Mediator.Send(command));
    }

    [Authorize(Roles = Roles.Technician)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPut(Router.TechnicianRouting.UpdateTechnicianProfile)]
    public async Task<IActionResult> UpdateTechnicianProfile([FromForm] UpdateTechnicianProfileCommand command)
    {
        return ToActionResult(await Mediator.Send(command));
    }
}
