using Fixy.Api.Base;
using Fixy.Api.Contracts.Routing;
using Fixy.Application.Features.Users.Commands.DeleteProfilePicture;
<<<<<<< HEAD
=======
using Fixy.Application.Features.Users.Queries.GetUserById;
>>>>>>> feature/MFA
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fixy.Api.Controllers;

[Authorize]
public class UsersController : AppControllerBase
{
<<<<<<< HEAD
=======
    [HttpGet(Router.UsersRouting.GetUserById)]
    public async Task<IActionResult> GetUserById([FromRoute] Guid Id)
    {
        return ToActionResult(await Mediator.Send(new GetUserByIdQuery(Id)));
    }

>>>>>>> feature/MFA
    [HttpDelete(Router.UsersRouting.DeleteProfilePicture)]
    public async Task<IActionResult> DeleteProfilePicture()
    {
        return ToActionResult(await Mediator.Send(new DeleteProfilePictureCommand()));
    }
}
