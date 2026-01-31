using Fixy.Api.Base;
using Fixy.Api.Contracts.Routing;
using Fixy.Application.Features.Payments.Commands.HandleStripePaymentSucceeded;
using Fixy.Application.Features.Payments.Commands.PayBooking;
using Fixy.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fixy.Api.Controllers;


public class PaymentsController : AppControllerBase
{
    [Authorize(Roles = Roles.Customer)]
    [HttpPost(Router.PaymentRouting.CreatePaymentIntent)]
    public async Task<IActionResult> CreateOrUpdatePaymentIntent([FromRoute] Guid BookingId)
    {
        return ToActionResult(await Mediator.Send(new PayBookingCommand(BookingId)));
    }

    [HttpPost(Router.PaymentRouting.WebHook)]
    public async Task<IActionResult> WebHook()
    {
        return ToActionResult(await Mediator.Send(new HandleStripePaymentSucceededCommand()));
    }
}
