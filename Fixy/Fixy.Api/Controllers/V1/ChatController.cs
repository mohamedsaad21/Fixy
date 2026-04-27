using Asp.Versioning;
using Fixy.Api.Contracts.Routing;
using Fixy.Api.Controllers.Common;
using Fixy.Application.Features.Chat.Commands.MarkMessagesAsRead;
using Fixy.Application.Features.Chat.Queries.GetChatMessages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fixy.Api.Controllers.V1;

[ApiVersion("1.0")]
[Authorize]
public class ChatController : AppControllerBase
{
    [HttpGet(Router.ChatRouting.GetMessages)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMessages([FromQuery] GetChatMessagesQuery query)
    {
        return ToActionResult(await Mediator.Send(query));
    }

    [HttpPost(Router.ChatRouting.MarkMessagesAsRead)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> MarkAsRead([FromRoute] Guid BookingId)
    {
        return ToActionResult(await Mediator.Send(new MarkMessagesAsReadCommand(BookingId)));
    }
}
