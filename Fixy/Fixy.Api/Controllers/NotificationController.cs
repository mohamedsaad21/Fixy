using Fixy.Api.Attributes;
using Fixy.Api.Base;
using Fixy.Api.Contracts.Routing;
using Fixy.Application.Features.Notifications.Commands.MarkAsRead;
using Fixy.Application.Features.Notifications.Commands.SaveFcmToken;
using Fixy.Application.Features.Notifications.Queries.GetNotifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Fixy.Api.Controllers;

[Authorize]
public class NotificationController : AppControllerBase
{

    //[RedisCache(60)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet(Router.NotificationsRouting.List)]
    public async Task<IActionResult> GetNotifications()
    {
        return ToActionResult(await Mediator.Send(new GetNotificationsQuery()));
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPut(Router.NotificationsRouting.MarkAsRead)]
    public async Task<IActionResult> MarkAsRead([FromRoute] Guid NotificationId)
    {
        return ToActionResult(await Mediator.Send(new MarkAsReadCommand(NotificationId)));
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPost(Router.NotificationsRouting.SaveFcmToken)]
    public async Task<IActionResult> SaveFcmToken([FromBody] SaveFcmTokenCommand command)
    {
        return ToActionResult(await Mediator.Send(command));
    }
}
