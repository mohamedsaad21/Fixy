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
    [HttpPost(Router.FeedbackRouting.SubmitCustomerFeedback)]
    public async Task<IActionResult> SubmitCustomerFeedbackAsync([FromForm] SubmitCustomerFeedbackCommand command)
    {
        return ToActionResult(await Mediator.Send(command));
    }

    [Authorize(Roles = Roles.Technician)]
    [HttpPost(Router.FeedbackRouting.SubmitTechnicianFeedback)]
    public async Task<IActionResult> SubmitTechnicianFeedbackAsync([FromForm] SubmitTechnicianFeedbackCommand command)
    {
        return ToActionResult(await Mediator.Send(command));
    }
}
