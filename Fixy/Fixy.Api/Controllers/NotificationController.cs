using Fixy.Api.Base;
using Fixy.Api.Contracts.Routing;
using Fixy.Application.Features.Notifications.Queries.GetNotifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Fixy.Api.Controllers;

[Authorize]
public class NotificationController : AppControllerBase
{
    [HttpGet(Router.NotificationsRouting.List)]
    public async Task<IActionResult> GetNotifications()
    {
        return ToActionResult(await Mediator.Send(new GetNotificationsQuery()));
    }
}
