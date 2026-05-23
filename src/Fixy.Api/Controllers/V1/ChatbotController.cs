using Asp.Versioning;
using Fixy.Api.Contracts.Routing;
using Fixy.Api.Controllers.Common;
using Fixy.Application.Features.Chatbot.Commands.SendPrompt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fixy.Api.Controllers.V1;

[ApiVersion("1.0")]
[Authorize]
public class ChatbotController : AppControllerBase
{
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPost(Router.ChatbotRouting.SendPrompt)]
    public async Task<IActionResult> SendPrompt([FromBody] SendPromptCommand command)
    {
        return ToActionResult(await Mediator.Send(command));
    }
}
