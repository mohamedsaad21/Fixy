using Fixy.Api.Base;
using Fixy.Api.Contracts.Routing;
using Fixy.Application.Features.ServiceRequests.Commands.CreateServiceRequest;
using Fixy.Application.Features.ServiceRequests.Queries.GetMyRequests;
using Fixy.Application.Features.ServiceRequests.Queries.GetServiceRequestList;
using Fixy.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fixy.Api.Controllers;

[Authorize]
public class ServiceRequestController : AppControllerBase
{
    [Authorize(Roles = Roles.Admin)]
    [HttpGet(Router.ServiceRequestRouting.ServiceRequestsList)]
    public async Task<IActionResult> GetServiceRequests()
    {
        return ToActionResult(await Mediator.Send(new GetServiceRequestListQuery()));
    }

    [Authorize(Roles = Roles.Customer)]
    [HttpGet(Router.ServiceRequestRouting.CustomerServiceRequestsList)]
    public async Task<IActionResult> GetCustomerServiceRequests()
    {
        return ToActionResult(await Mediator.Send(new GetMyRequestsQuery()));
    }

    [Authorize(Roles = Roles.Customer)]
    [HttpPost(Router.ServiceRequestRouting.Create)]
    public async Task<IActionResult> CreateServiceRequest([FromForm] CreateServiceRequestCommand command)
    {
        return ToActionResult(await Mediator.Send(command));
    }
}
