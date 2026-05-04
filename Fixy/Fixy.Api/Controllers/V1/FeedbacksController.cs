﻿using Asp.Versioning;
using Fixy.Api.Attributes;
using Fixy.Api.Contracts.Routing;
using Fixy.Api.Controllers.Common;
using Fixy.Application.Features.Feedbacks.Commands.SubmitCustomerFeedback;
using Fixy.Application.Features.Feedbacks.Commands.SubmitTechnicianFeedback;
using Fixy.Application.Features.Feedbacks.Queries.GetPendingCustomerFeedbackStatus;
using Fixy.Application.Features.Feedbacks.Queries.GetPendingTechnicianFeedbackStatus;
using Fixy.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fixy.Api.Controllers.V1;

[ApiVersion("1.0")]
public class FeedbacksController : AppControllerBase
{
    [Authorize(Roles = Roles.Customer)]
    [RequireActiveCustomer]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPost(Router.FeedbackRouting.SubmitCustomerFeedback)]
    public async Task<IActionResult> SubmitCustomerFeedbackAsync([FromForm] SubmitCustomerFeedbackCommand command)
    {
        return ToActionResult(await Mediator.Send(command));
    }

    [Authorize(Roles = Roles.Technician)]
    [RequireActiveTechnician]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPost(Router.FeedbackRouting.SubmitTechnicianFeedback)]
    public async Task<IActionResult> SubmitTechnicianFeedbackAsync([FromForm] SubmitTechnicianFeedbackCommand command)
    {
        return ToActionResult(await Mediator.Send(command));
    }

    [Authorize(Roles = Roles.Customer)]
    [RequireActiveCustomer]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet(Router.FeedbackRouting.GetPendingCustomerFeedbackStatus)]
    public async Task<IActionResult> GetPendingCustomerFeedbackStatus()
    {
        return ToActionResult(await Mediator.Send(new GetPendingCustomerFeedbackStatusQuery()));
    }

    [Authorize(Roles = Roles.Technician)]
    [RequireActiveTechnician]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet(Router.FeedbackRouting.GetPendingTechnicianFeedbackStatus)]
    public async Task<IActionResult> GetPendingTechnicianFeedbackStatus()
    {
        return ToActionResult(await Mediator.Send(new GetPendingTechnicianFeedbackStatusQuery()));
    }
}
