using Fixy.Api.Base;
using Fixy.Api.Contracts.Routing;
using Fixy.Application.Features.ServiceRequests.Commands.CreateServiceRequest;
using Fixy.Application.Features.ServiceRequests.Queries.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fixy.Api.Controllers;

public class ServiceRequestController : AppControllerBase
{
    [HttpGet(Router.ServiceRequestRouting.List)]
    public async Task<IActionResult> GetServiceRequests()
    {
        return ToActionResult(await Mediator.Send(new GetRequestsListQuery()));
    }

    [HttpPost(Router.ServiceRequestRouting.Create)]
    public async Task<IActionResult> CreateServiceRequest([FromForm] CreateServiceRequestCommand command)
    {
        return ToActionResult(await Mediator.Send(command));
    }
}
