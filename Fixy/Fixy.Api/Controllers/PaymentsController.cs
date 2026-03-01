using Fixy.Api.Base;
using Fixy.Api.Contracts.Routing;
using Fixy.Application.Features.Payments.Commands.ConfirmCashReceipt;
using Fixy.Application.Features.Payments.Commands.CreatePayment;
using Fixy.Application.Features.Payments.Commands.PayCommission;
using Fixy.Application.Features.Payments.Commands.ProcessCallback;
using Fixy.Application.Features.Payments.Queries.GetPendingCommissions;
using Fixy.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fixy.Api.Controllers;

[Authorize]
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
    [HttpGet(Router.PaymentRouting.Callback)]
    public async Task<IActionResult> PaymobCallback()
    {
        try
        {
            var query = Request.Query;

            // Validate required parameters
            if (!query.ContainsKey("id") || !query.ContainsKey("merchant_order_id") || !query.ContainsKey("hmac"))
            {
                return Ok(new { status = "error", message = "Missing parameters" });
            }

            // Process callback
            return ToActionResult(await Mediator.Send(new ProcessCallbackCommand(query)));

        }
        catch (Exception ex)
        {
            return Ok(new { status = "error" });
        }
    }

    [Authorize(Roles = Roles.Technician)]
    [HttpPost(Router.PaymentRouting.confirmCashReceipt)]
    public async Task<IActionResult> ConfirmCashReceipt([FromRoute] Guid BookingId)
    {
        return ToActionResult(await Mediator.Send(new ConfirmCashReceiptCommand(BookingId)));
    }

    [Authorize(Roles = Roles.Technician)]
    [HttpPost(Router.PaymentRouting.PayCommissions)]
    public async Task<IActionResult> PayCommissions([FromBody] PayCommissionCommand command)
    {
        return ToActionResult(await Mediator.Send(command));
    }

    [Authorize(Roles = Roles.Technician)]
    [HttpGet(Router.PaymentRouting.GetPendingCommissions)]
    public async Task<IActionResult> GetPendingCommissions()
    {
        return ToActionResult(await Mediator.Send(new GetPendingCommissionsQuery()));
    }
}
