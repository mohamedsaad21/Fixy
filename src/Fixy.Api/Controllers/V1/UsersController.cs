using Asp.Versioning;
using Fixy.Api.Attributes;
using Fixy.Api.Contracts.Routing;
using Fixy.Api.Controllers.Common;
using Fixy.Application.Features.Users.Commands.DeleteProfilePicture;
using Fixy.Application.Features.Users.Commands.EditUserProfile;
using Fixy.Application.Features.Users.Queries.GetCurrentUser;
using Fixy.Application.Features.Users.Queries.GetUserProfileById;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fixy.Api.Controllers.V1;

[ApiVersion("1.0")]
[Authorize]
public class UsersController : AppControllerBase
{
    //[RedisCache(3)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet(Router.UsersRouting.GetUserProfileById)]
    public async Task<IActionResult> GetUserById([FromRoute] Guid Id)
    {
        return ToActionResult(await Mediator.Send(new GetUserProfileByIdQuery(Id)));
    }

    //[RedisCache(3)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet(Router.UsersRouting.Me)]
    public async Task<IActionResult> GetCurrentUser()
    {
        return ToActionResult(await Mediator.Send(new GetCurrentUserQuery()));
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
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPut(Router.UsersRouting.EditUserProfile)]
    public async Task<IActionResult> EditUserProfile([FromForm] EditUserProfileCommand command)
    {
        return ToActionResult(await Mediator.Send(command));
    }
}
