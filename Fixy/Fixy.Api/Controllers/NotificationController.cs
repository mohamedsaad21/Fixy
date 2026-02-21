using Fixy.Api.Base;
using Fixy.Api.Contracts.Routing;
using Fixy.Application.Features.Notifications.Commands.MarkAsRead;
using Fixy.Application.Features.Notifications.Commands.SendNotification;
using Fixy.Application.Features.Notifications.Queries.GetNotifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Fixy.Api.Controllers;

[Authorize]
public class NotificationController : AppControllerBase
{
    [HttpPost(Router.NotificationsRouting.SendNotification)]
    public async Task<IActionResult> SendNotification([FromBody] SendNotificationCommand command)
    {
        return ToActionResult(await Mediator.Send(command));
    }

    [HttpGet(Router.NotificationsRouting.List)]
    public async Task<IActionResult> GetNotifications()
    {
        return ToActionResult(await Mediator.Send(new GetNotificationsQuery()));
    }

    [HttpPut(Router.NotificationsRouting.MarkAsRead)]
    public async Task<IActionResult> MarkAsRead([FromRoute] Guid NotificationId)
    {
        return ToActionResult(await Mediator.Send(new MarkAsReadCommand(NotificationId)));
    }
}
