using Fixy.Api.Base;
using Fixy.Api.Contracts.Routing;
using Fixy.Application.Features.Admin.Commands.ApproveTechnician;
using Fixy.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fixy.Api.Controllers;

[Authorize(Roles = Roles.Admin)]
public class AdminController : AppControllerBase
{
    [HttpPost(Router.AdminRouting.ApproveTechnician)]
    public async Task<IActionResult> ApproveTechnician([FromRoute] Guid TechnicianId)
    {
        return ToActionResult(await Mediator.Send(new ApproveTechnicianCommand(TechnicianId)));
    }
}
