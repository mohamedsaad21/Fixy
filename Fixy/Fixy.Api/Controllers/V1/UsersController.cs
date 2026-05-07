using Asp.Versioning;
using Fixy.Api.Contracts.Routing;
using Fixy.Api.Controllers.Common;
using Fixy.Application.Features.Users.Commands.DeleteProfilePicture;
using Fixy.Application.Features.Users.Commands.EditLanguage;
using Fixy.Application.Features.Users.Queries.GetUserProfileById;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fixy.Api.Controllers.V1;

[ApiVersion("1.0")]
[Authorize]
public class UsersController : AppControllerBase
{
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet(Router.UsersRouting.GetUserProfileById)]
    public async Task<IActionResult> GetUserById([FromRoute] Guid Id)
    {
        return ToActionResult(await Mediator.Send(new GetUserProfileByIdQuery(Id)));
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpDelete(Router.UsersRouting.DeleteProfilePicture)]
    public async Task<IActionResult> DeleteProfilePicture()
    {
        return ToActionResult(await Mediator.Send(new DeleteProfilePictureCommand()));
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPut(Router.UsersRouting.EditLanguage)]
    public async Task<IActionResult> EditLanguage([FromQuery] EditLanguageCommand command)
    {
        return ToActionResult(await Mediator.Send(command));
    }
}
