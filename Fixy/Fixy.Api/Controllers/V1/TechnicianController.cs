using Fixy.Api.Attributes;
using Asp.Versioning;
using Fixy.Api.Contracts.Routing;
using Fixy.Api.Controllers.Common;
using Fixy.Application.Features.Technicians.Commands.UpdateTechnicianLocation;
using Fixy.Application.Features.Technicians.Commands.UpdateTechnicianProfile;
using Fixy.Application.Features.Technicians.Queries.GetAvailableServiceRequestsForTechnician;
using Fixy.Application.Features.Technicians.Queries.GetCustomerProfileForTechnicians;
using Fixy.Application.Features.Technicians.Queries.GetServiceRequestById;
using Fixy.Application.Features.Technicians.Queries.GetTechnicianById;
using Fixy.Application.Features.Technicians.Queries.GetTechnicianProfileForCustomers;
using Fixy.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fixy.Api.Controllers.V1;

[ApiVersion("1.0")]
[Authorize]
public class TechnicianController : AppControllerBase
{
    //[RedisCache(3)]
    [Authorize(Roles = $"{Roles.Technician},{Roles.Admin}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet(Router.TechnicianRouting.GetById)]
    public async Task<IActionResult> GetTechnicianById([FromRoute] Guid Id)
    {
        return ToActionResult(await Mediator.Send(new GetTechnicianByIdQuery(Id)));
    }

    //[RedisCache(3)]
    [Authorize(Roles = $"{Roles.Customer},{Roles.Admin}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet(Router.TechnicianRouting.GetTechnicianProfileForCustomers)]
    public async Task<IActionResult> GetTechnicianProfileForCustomers([FromRoute] Guid TechnicianId)
    {
        return ToActionResult(await Mediator.Send(new GetTechnicianProfileForCustomersQuery(TechnicianId)));
    }

    //[RedisCache(3)]
    [RequireActiveTechnician]
    [Authorize(Roles = $"{Roles.Technician},{Roles.Admin}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet(Router.TechnicianRouting.GetCustomerProfileForTechnicians)]
    public async Task<IActionResult> GetCustomerProfileForTechnicians([FromRoute] Guid CustomerId)
    {
        return ToActionResult(await Mediator.Send(new GetCustomerProfileForTechniciansQuery(CustomerId)));
    }

    //[RedisCache(3)]
    [RequireActiveTechnician]
    [Authorize(Roles = $"{Roles.Technician},{Roles.Admin}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet(Router.TechnicianRouting.TechnicianServiceRequestsList)]
    public async Task<IActionResult> GetTechnicianAvailableServiceRequests([FromQuery] GetAvailableServiceRequestsForTechnicianQuery query)
    {
        return ToActionResult(await Mediator.Send(query));
    }

    //[RedisCache(3)]
    [RequireActiveTechnician]
    [Authorize(Roles = $"{Roles.Technician},{Roles.Admin}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet(Router.TechnicianRouting.ServiceRequestById)]
    public async Task<IActionResult> GetServiceRequestById([FromRoute] GetTechnicianServiceRequestByIdQuery query)
    {
        return ToActionResult(await Mediator.Send(query));
    }

    [RequireActiveTechnician]
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
