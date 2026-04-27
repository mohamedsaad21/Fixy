using Asp.Versioning;
using Fixy.Api.Contracts.Routing;
using Fixy.Api.Controllers.Common;
using Fixy.Application.Features.Authentication.Commands.ChangePassword;
using Fixy.Application.Features.Authentication.Commands.ConfirmEmail;
using Fixy.Application.Features.Authentication.Commands.DisableTwoFactor;
using Fixy.Application.Features.Authentication.Commands.EnableTwoFactor;
using Fixy.Application.Features.Authentication.Commands.RefreshToken;
using Fixy.Application.Features.Authentication.Commands.RegisterCustomer;
using Fixy.Application.Features.Authentication.Commands.RegisterTechnician;
using Fixy.Application.Features.Authentication.Commands.ResetPassword;
using Fixy.Application.Features.Authentication.Commands.RevokeToken;
using Fixy.Application.Features.Authentication.Commands.SendConfirmEmail;
using Fixy.Application.Features.Authentication.Commands.SendResetPassword;
using Fixy.Application.Features.Authentication.Commands.SignIn;
using Fixy.Application.Features.Authentication.Commands.VerifyOTP;
using Fixy.Application.Features.Authentication.Queries.ConfirmResetPassword;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fixy.Api.Controllers.V1;

[ApiVersion("1.0")]
public class AuthenticationController : AppControllerBase
{
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost(Router.AuthenticationRouting.RegisterTechnician)]
    public async Task<IActionResult> RegisterTechnicianAsync([FromForm] RegisterTechnicianCommand command)
    {
        return ToActionResult(await Mediator.Send(command));
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost(Router.AuthenticationRouting.RegisterCustomer)]
    public async Task<IActionResult> RegisterCustomerAsync([FromForm] RegisterCustomerCommand command)
    {
        return ToActionResult(await Mediator.Send(command));
    }


    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost(Router.AuthenticationRouting.SignIn)]
    public async Task<IActionResult> SignInAsync([FromForm] SignInCommand command)
    {       
        return ToActionResult(await Mediator.Send(command));
    }

    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPost(Router.AuthenticationRouting.Enable2FA)]
    public async Task<IActionResult> Enable2FA([FromForm] EnableTwoFactorCommand command)
    {
        return ToActionResult(await Mediator.Send(command));
    }

    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPost(Router.AuthenticationRouting.Disable2FA)]
    public async Task<IActionResult> Disable2FA([FromForm] DisableTwoFactorCommand command)
    {
        return ToActionResult(await Mediator.Send(command));
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPost(Router.AuthenticationRouting.VerifyOtp)]
    public async Task<IActionResult> VerifyOtp([FromForm] VerifyOTPCommand command)
    {
        return ToActionResult(await Mediator.Send(command));
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPost(Router.AuthenticationRouting.SendConfirmEmail)]
    public async Task<IActionResult> SendConfirmEmailAsync([FromForm] SendConfirmEmailCommand command)
    {
        return ToActionResult(await Mediator.Send(command));
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPost(Router.AuthenticationRouting.ConfirmEmail)]
    public async Task<IActionResult> ConfirmEmailAsync([FromForm] ConfirmEmailCommand command)
    {
        return ToActionResult(await Mediator.Send(command));
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost(Router.AuthenticationRouting.RefreshToken)]
    public async Task<IActionResult> RefreshTokenAsync()
    {
        return ToActionResult(await Mediator.Send(new RefreshTokenCommand()));
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost(Router.AuthenticationRouting.RevokeToken)]
    public async Task<IActionResult> RevokeTokenAsync()
    {
        return ToActionResult(await Mediator.Send(new RevokeTokenCommand()));
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPost(Router.AuthenticationRouting.SendResetPassword)]
    public async Task<IActionResult> SendResetPasswordAsync([FromForm] SendResetPasswordCommand command)
    {
        return ToActionResult(await Mediator.Send(command));
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet(Router.AuthenticationRouting.ConfirmResetPassword)]
    public async Task<IActionResult> ConfirmResetPasswordAsync([FromQuery] ConfirmResetPasswordQuery query)
    {
        return ToActionResult(await Mediator.Send(query));
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPost(Router.AuthenticationRouting.ResetPassword)]
    public async Task<IActionResult> ResetPasswordAsync([FromForm] ResetPasswordCommand command)
    {
        return ToActionResult(await Mediator.Send(command));
    }

    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPut(Router.AuthenticationRouting.ChangePassword)]
    public async Task<IActionResult> ChangePasswordAsync([FromForm] ChangePasswordCommand command)
    {
        return ToActionResult(await Mediator.Send(command));
    }
}