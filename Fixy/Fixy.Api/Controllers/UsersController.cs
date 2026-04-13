using Fixy.Api.Base;
using Fixy.Api.Contracts.Routing;
using Fixy.Application.Features.Users.Commands.DeleteProfilePicture;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fixy.Api.Controllers;

[Authorize]
public class UsersController : AppControllerBase
{
    [HttpDelete(Router.UsersRouting.DeleteProfilePicture)]
    public async Task<IActionResult> DeleteProfilePicture()
    {
        return ToActionResult(await Mediator.Send(new DeleteProfilePictureCommand()));
    }
}
