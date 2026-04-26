using Fixy.Api.Attributes;
using Fixy.Api.Base;
using Fixy.Api.Contracts.Routing;
using Fixy.Application.Features.Feedbacks.Commands.SubmitCustomerFeedback;
using Fixy.Application.Features.Feedbacks.Commands.SubmitTechnicianFeedback;
using Fixy.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fixy.Api.Controllers;

public class FeedbacksController : AppControllerBase
{
    [Authorize(Roles = Roles.Customer)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPost(Router.FeedbackRouting.SubmitCustomerFeedback)]
    public async Task<IActionResult> SubmitCustomerFeedbackAsync([FromForm] SubmitCustomerFeedbackCommand command)
    {
        return ToActionResult(await Mediator.Send(command));
    }

    [RequireActiveTechnician]
    [Authorize(Roles = Roles.Technician)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPost(Router.FeedbackRouting.SubmitTechnicianFeedback)]
    public async Task<IActionResult> SubmitTechnicianFeedbackAsync([FromForm] SubmitTechnicianFeedbackCommand command)
    {
        return ToActionResult(await Mediator.Send(command));
    }
}
