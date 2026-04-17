using Fixy.Api.Base;
using Fixy.Api.Contracts.Routing;
using Fixy.Application.Features.Payments.Commands.ConfirmCashReceipt;
using Fixy.Application.Features.Payments.Commands.CreatePayment;
using Fixy.Application.Features.Payments.Commands.PayCommission;
using Fixy.Application.Features.Payments.Commands.ProcessCallback;
using Fixy.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fixy.Api.Controllers;

[Authorize]
public class PaymentsController : AppControllerBase
{
    [Authorize(Roles = Roles.Customer)]
    [HttpPost(Router.PaymentRouting.Create)]
    public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentCommand command)
    {
        return ToActionResult(await Mediator.Send(command));
    }

    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
}
