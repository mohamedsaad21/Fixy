using Asp.Versioning;
using Fixy.Api.Contracts.Routing;
using Fixy.Api.Controllers.Common;
using Fixy.Application.Features.Chat.Queries.GetChatMessages;
using Fixy.Application.Features.Chatbot.Commands.SendPrompt;
using Fixy.Application.Features.Chatbot.Queries.GetChatbotHistory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fixy.Api.Controllers.V1;

[ApiVersion("1.0")]
[Authorize]
public class ChatbotController : AppControllerBase
{
    [HttpGet(Router.ChatbotRouting.GetChatHistory)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetChatHistory([FromQuery] GetChatbotHistoryQuery query)
    {
        return ToActionResult(await Mediator.Send(query));
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPost(Router.ChatbotRouting.SendMessage)]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageCommand command)
    {
        return ToActionResult(await Mediator.Send(command));
    }
}
