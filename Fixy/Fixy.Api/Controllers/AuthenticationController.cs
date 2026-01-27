using Fixy.Api.Base;
using Fixy.Api.Contracts.Routing;
using Fixy.Application.Features.Authentication.Commands.Models;
using Fixy.Application.Features.Authentication.Queries.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fixy.Api.Controllers;

public class AuthenticationController : AppControllerBase
{
    [HttpPost(Router.AuthenticationRouting.RegisterTechnician)]
    public async Task<IActionResult> RegisterTechnicianAsync([FromForm] RegisterTechnicianCommand command)
    {
        return ToActionResult(await Mediator.Send(command));
    }

    [HttpPost(Router.AuthenticationRouting.RegisterCustomer)]
    public async Task<IActionResult> RegisterCustomerAsync([FromBody] RegisterCustomerCommand command)
    {
        return ToActionResult(await Mediator.Send(command));
    }

    [HttpPost(Router.AuthenticationRouting.SignIn)]
    public async Task<IActionResult> SignInAsync([FromForm] SignInCommand command)
    {       
        return ToActionResult(await Mediator.Send(command));
    }

    [HttpGet(Router.AuthenticationRouting.CustomersList)]
    public async Task<IActionResult> GetCustomersAsync()
    {
        return ToActionResult(await Mediator.Send(new GetCustomersQuery()));
    }

    [HttpPost(Router.AuthenticationRouting.SendConfirmEmail)]
    public async Task<IActionResult> SendConfirmEmailAsync([FromForm] SendConfirmEmailCommand command)
    {
        return ToActionResult(await Mediator.Send(command));
    }

    [HttpPost(Router.AuthenticationRouting.ConfirmEmail)]
    public async Task<IActionResult> ConfirmEmailAsync([FromForm] ConfirmEmailCommand command)
    {
        return ToActionResult(await Mediator.Send(command));
    }

    [HttpPost(Router.AuthenticationRouting.RefreshToken)]
    public async Task<IActionResult> RefreshTokenAsync()
    {
        return ToActionResult(await Mediator.Send(new RefreshTokenCommand()));
    }

    [HttpPost(Router.AuthenticationRouting.RevokeToken)]
    public async Task<IActionResult> RevokeTokenAsync([FromBody] RevokeTokenCommand command)
    {
        return ToActionResult(await Mediator.Send(command));
    }

    [HttpPost(Router.AuthenticationRouting.SendResetPassword)]
    public async Task<IActionResult> SendResetPasswordAsync([FromForm] SendResetPasswordCommand command)
    {
        return ToActionResult(await Mediator.Send(command));
    }

    [HttpGet(Router.AuthenticationRouting.ConfirmResetPassword)]
    public async Task<IActionResult> ConfirmResetPasswordAsync([FromQuery] ConfirmResetPasswordQuery query)
    {
        return ToActionResult(await Mediator.Send(query));
    }

    [HttpPost(Router.AuthenticationRouting.ResetPassword)]
    public async Task<IActionResult> ResetPasswordAsync([FromForm] ResetPasswordCommand command)
    {
        return ToActionResult(await Mediator.Send(command));
    }
}