using Fixy.Api.Base;
using Fixy.Application.Features.Authentication.Commands.Models;
using Fixy.Application.Features.Authentication.Queries.Models;
using Fixy.Domain.AppMetaData;
using Microsoft.AspNetCore.Mvc;

namespace Fixy.Api.Controllers;

public class AuthenticationController : AppControllerBase
{
    [HttpPost(Router.AuthenticationRouting.RegisterCustomer)]
    public async Task<IActionResult> RegisterCustomerAsync([FromBody] RegisterCustomerCommand command)
    {
        return ToActionResult(await Mediator.Send(command));
    }

    [HttpPost(Router.AuthenticationRouting.SignIn)]
    public async Task<IActionResult> SignInAsync([FromQuery] SignInCommand command)
    {
        return ToActionResult(await Mediator.Send(command));
    }

    [HttpGet(Router.AuthenticationRouting.CustomersList)]
    public async Task<IActionResult> GetCustomersAsync()
    {
        return ToActionResult(await Mediator.Send(new GetCustomersQuery()));
    }

    [HttpGet(Router.AuthenticationRouting.ConfirmEmail)]
    public async Task<IActionResult> ConfirmEmailAsync([FromQuery] ConfirmEmailQuery query)
    {
        return ToActionResult(await Mediator.Send(query));
    }
}
