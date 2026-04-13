using Fixy.Api.Attributes;
using Fixy.Api.Base;
using Fixy.Api.Contracts.Routing;
using Fixy.Application.Features.ServiceRequests.Commands.AddServiceRequestImages;
using Fixy.Application.Features.ServiceRequests.Commands.CreateServiceRequest;
using Fixy.Application.Features.ServiceRequests.Commands.DeleteServiceRequestImages;
using Fixy.Application.Features.ServiceRequests.Commands.EditServiceRequest;
using Fixy.Application.Features.ServiceRequests.Queries.GetMyRequests;
using Fixy.Application.Features.ServiceRequests.Queries.GetServiceRequestById;
using Fixy.Application.Features.ServiceRequests.Queries.GetServiceRequestList;
using Fixy.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fixy.Api.Controllers;

[Authorize]
public class ServiceRequestController : AppControllerBase
{
<<<<<<< HEAD
    [RedisCache(60)]
=======
    //[RedisCache(60)]
>>>>>>> feature/MFA
    [Authorize(Roles = Roles.Admin)]
    [HttpGet(Router.ServiceRequestRouting.ServiceRequestsList)]
    public async Task<IActionResult> GetServiceRequests()
    {
        return ToActionResult(await Mediator.Send(new GetServiceRequestListQuery()));
    }

<<<<<<< HEAD
    [RedisCache(60)]
=======
    //[RedisCache(60)]
>>>>>>> feature/MFA
    [Authorize(Roles = Roles.Customer)]
    [HttpGet(Router.ServiceRequestRouting.CustomerServiceRequestsPaginated)]
    public async Task<IActionResult> GetCustomerServiceRequestsPaginated([FromQuery] GetMyRequestsQuery query)
    {
        return ToActionResult(await Mediator.Send(query));
    }

<<<<<<< HEAD
    [RedisCache(60)]
=======
    //[RedisCache(60)]
>>>>>>> feature/MFA
    [Authorize(Roles = Roles.Customer)]
    [HttpGet(Router.ServiceRequestRouting.CustomerServiceRequestById)]
    public async Task<IActionResult> GetCustomerServiceRequestById([FromRoute] Guid Id)
    {
        return ToActionResult(await Mediator.Send(new GetServiceRequestByIdQuery(Id)));
    }

    [Authorize(Roles = Roles.Customer)]
    [HttpPost(Router.ServiceRequestRouting.Create)]
    public async Task<IActionResult> CreateServiceRequest([FromForm] CreateServiceRequestCommand command)
    {
        return ToActionResult(await Mediator.Send(command));
    }

    [Authorize(Roles = Roles.Customer)]
    [HttpPut(Router.ServiceRequestRouting.Edit)]
    public async Task<IActionResult> EditServiceRequest([FromForm] EditServiceRequestCommand command)
    {
        return ToActionResult(await Mediator.Send(command));
    }

    [Authorize(Roles = Roles.Customer)]
    [HttpPost(Router.ServiceRequestRouting.AddServiceRequestImages)]
    public async Task<IActionResult> AddServiceRequestImages([FromForm] AddServiceRequestImagesCommand command)
    {
        return ToActionResult(await Mediator.Send(command));
    }

    [Authorize(Roles = Roles.Customer)]
    [HttpDelete(Router.ServiceRequestRouting.DeleteServiceRequestImageById)]
    public async Task<IActionResult> DeleteServiceRequestImages([FromRoute] Guid ImageId)
    {
        return ToActionResult(await Mediator.Send(new DeleteServiceRequestImageByIdCommand(ImageId)));
    }
}
