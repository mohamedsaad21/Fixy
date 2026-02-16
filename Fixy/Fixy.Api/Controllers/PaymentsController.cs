using Fixy.Api.Base;
using Fixy.Api.Contracts.Routing;
using Fixy.Application.Common.DTOs.Payment;
using Fixy.Application.Features.Payments.Commands.CreatePayment;
using Fixy.Application.Features.Payments.Commands.ProcessCallback;
using Fixy.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fixy.Api.Controllers;

public class PaymentsController : AppControllerBase
{
    /// <summary>
    /// Create payment for a booking
    /// Customer endpoint
    /// </summary>
    [Authorize(Roles = Roles.Customer)]
    [HttpPost(Router.PaymentRouting.Create)]
    public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentCommand command)
    {
        return ToActionResult(await Mediator.Send(command));
    }

    /// <summary>
    /// Paymob callback endpoint
    /// MUST be public (no authorization)
    /// </summary>
    [AllowAnonymous]
    [HttpPost(Router.PaymentRouting.Callback)]
    public async Task<IActionResult> PaymobCallback([FromBody] PaymobCallbackDto callback)
    {
        return ToActionResult(await Mediator.Send(new ProcessCallbackCommand(callback)));
    }

}
