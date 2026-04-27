using Asp.Versioning;
using Fixy.Api.Contracts.Routing;
using Fixy.Api.Controllers.Common;
using Fixy.Application.Features.Users.Commands.DeleteProfilePicture;
using Fixy.Application.Features.Users.Queries.GetUserById;
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
    [HttpGet(Router.UsersRouting.GetUserById)]
    public async Task<IActionResult> GetUserById([FromRoute] Guid Id)
    {
        return ToActionResult(await Mediator.Send(new GetUserByIdQuery(Id)));
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpDelete(Router.UsersRouting.DeleteProfilePicture)]
    public async Task<IActionResult> DeleteProfilePicture()
    {
        return ToActionResult(await Mediator.Send(new DeleteProfilePictureCommand()));
    }
}
