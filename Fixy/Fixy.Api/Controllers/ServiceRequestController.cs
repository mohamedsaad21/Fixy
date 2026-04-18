using Fixy.Api.Attributes;
using Fixy.Api.Base;
using Fixy.Api.Contracts.Routing;
using Fixy.Application.Features.ServiceRequests.Commands.AddServiceRequestImages;
using Fixy.Application.Features.ServiceRequests.Commands.CreateServiceRequest;
using Fixy.Application.Features.ServiceRequests.Commands.DeleteServiceRequestImages;
using Fixy.Application.Features.ServiceRequests.Commands.EditServiceRequest;
using Fixy.Application.Features.ServiceRequests.Queries.GetMyRequests;
using Fixy.Application.Features.ServiceRequests.Queries.GetServiceRequestById;
using Fixy.Application.Features.ServiceRequests.Queries.GetServiceRequestPaginaredList;
using Fixy.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fixy.Api.Controllers;

[Authorize]
public class ServiceRequestController : AppControllerBase
{
    //[RedisCache(60)]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet(Router.ServiceRequestRouting.ServiceRequestPaginatedList)]
    public async Task<IActionResult> GetServiceRequests([FromQuery] GetServiceRequestPaginaredListQuery query)
    {
        return ToActionResult(await Mediator.Send(query));
    }

    //[RedisCache(60)]
    [Authorize(Roles = Roles.Customer)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet(Router.ServiceRequestRouting.CustomerServiceRequestsPaginated)]
    public async Task<IActionResult> GetCustomerServiceRequestsPaginated([FromQuery] GetMyRequestPaginatedListQuery query)
    {
        return ToActionResult(await Mediator.Send(query));
    }

    //[RedisCache(60)]
    [Authorize(Roles = Roles.Customer)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet(Router.ServiceRequestRouting.CustomerServiceRequestById)]
    public async Task<IActionResult> GetCustomerServiceRequestById([FromRoute] Guid Id)
    {
        return ToActionResult(await Mediator.Send(new GetCustomerServiceRequestByIdQuery(Id)));
    }

    [Authorize(Roles = Roles.Customer)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPost(Router.ServiceRequestRouting.Create)]
    public async Task<IActionResult> CreateServiceRequest([FromForm] CreateServiceRequestCommand command)
    {
        return ToActionResult(await Mediator.Send(command));
    }

    [Authorize(Roles = Roles.Customer)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPut(Router.ServiceRequestRouting.Edit)]
    public async Task<IActionResult> EditServiceRequest([FromForm] EditServiceRequestCommand command)
    {
        return ToActionResult(await Mediator.Send(command));
    }

    [Authorize(Roles = Roles.Customer)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPost(Router.ServiceRequestRouting.AddServiceRequestImages)]
    public async Task<IActionResult> AddServiceRequestImages([FromForm] AddServiceRequestImagesCommand command)
    {
        return ToActionResult(await Mediator.Send(command));
    }

    [Authorize(Roles = Roles.Customer)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpDelete(Router.ServiceRequestRouting.DeleteServiceRequestImageById)]
    public async Task<IActionResult> DeleteServiceRequestImages([FromRoute] Guid ImageId)
    {
        return ToActionResult(await Mediator.Send(new DeleteServiceRequestImageByIdCommand(ImageId)));
    }
}
