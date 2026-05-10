using Asp.Versioning;
using Fixy.Api.Attributes;
using Fixy.Api.Contracts.Routing;
using Fixy.Api.Controllers.Common;
using Fixy.Application.Features.Notifications.Commands.MarkAsRead;
using Fixy.Application.Features.Notifications.Commands.SaveFcmToken;
using Fixy.Application.Features.Notifications.Queries.GetNotifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Fixy.Api.Controllers.V1;

[ApiVersion("1.0")]
[Authorize]
public class NotificationController : AppControllerBase
{

    //[RedisCache(3)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet(Router.NotificationsRouting.PaginatedList)]
    public async Task<IActionResult> GetNotifications([FromQuery] GetNotificationsQuery query)
    {
        return ToActionResult(await Mediator.Send(query));
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
