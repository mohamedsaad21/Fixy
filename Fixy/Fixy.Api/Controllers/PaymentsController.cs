using Fixy.Api.Base;
using Fixy.Api.Contracts.Routing;
using Fixy.Application.Features.Payments.Commands.ConfirmCashReceipt;
using Fixy.Application.Features.Payments.Commands.PayBooking;
using Fixy.Application.Features.Payments.Commands.PayCommission;
using Fixy.Application.Features.Payments.Commands.ProcessCallback;
using Fixy.Application.Features.Payments.Commands.RequestWithdrawal;
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
    public async Task<IActionResult> CreatePayment([FromBody] PayBookingCommand command)
    {
        return ToActionResult(await Mediator.Send(command));
    }

    /// <summary>
    /// Paymob callback endpoint
    /// MUST be public (no authorization)
    /// </summary>
    [AllowAnonymous]
    [HttpPost(Router.PaymentRouting.Callback)]
    public async Task<IActionResult> PaymobCallback()
    {
        try
        {
            var result = await Mediator.Send(new ProcessCallbackCommand());

            if (!result.IsSuccess)
                return BadRequest(result);

            return Redirect("https://drive.google.com/drive/folders/1XATpVuqb4YR6FWURsu15zOOPZqAdVcEw");
        }
        catch (Exception ex)
        {
            //return Redirect($"https://your-frontend.com/payment/failed?error={ex.Message}");
            return Redirect("https://www.google.com/");
        }
    }

    [Authorize(Roles = Roles.Technician)]
    [HttpPost(Router.PaymentRouting.confirmCashReceipt)]
    public async Task<IActionResult> ConfirmCashReceipt([FromRoute] Guid BookingId)
    {
        return ToActionResult(await Mediator.Send(new ConfirmCashReceiptCommand(BookingId)));
    }

    [Authorize(Roles = Roles.Technician)]
    [HttpPost(Router.PaymentRouting.Withdraw)]
    public async Task<IActionResult> Withdraw([FromForm] RequestWithdrawalCommand command)
    {
        return ToActionResult(await Mediator.Send(command));
    }

    [Authorize(Roles = Roles.Technician)]
    [HttpPost(Router.PaymentRouting.PayCommissions)]
    public async Task<IActionResult> PayCommissions([FromBody] PayCommissionCommand command)
    {
        return ToActionResult(await Mediator.Send(command));
    }
}
