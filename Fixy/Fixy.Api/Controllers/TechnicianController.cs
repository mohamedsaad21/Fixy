using Fixy.Api.Base;
using Fixy.Api.Contracts.Routing;
using Fixy.Application.Features.Technicians.Commands.UpdateTechnicianLocation;
using Fixy.Application.Features.Technicians.Queries.GetTechnicianAvailableRequests;
using Fixy.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fixy.Api.Controllers;

[Authorize(Roles = Roles.Technician)]
public class TechnicianController : AppControllerBase
{
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet(Router.TechnicianRouting.TechnicianServiceRequestsList)]
    public async Task<IActionResult> GetTechnicianAvailableServiceRequests()
    {
        return ToActionResult(await Mediator.Send(new GetTechnicianAvailableRequestsQuery()));
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPost(Router.TechnicianRouting.Location)]
    public async Task<IActionResult> UpdateTechnicianLocation([FromQuery] UpdateTechnicianLocationCommand command)
    {
        return ToActionResult(await Mediator.Send(command));
    }
}
