using Fixy.Api.Attributes;
using Fixy.Api.Base;
using Fixy.Api.Contracts.Routing;
using Fixy.Application.Features.Bookings.Commands.ApproveBookingPriceChange;
using Fixy.Application.Features.Bookings.Commands.ConfirmBookingCompletion;
using Fixy.Application.Features.Bookings.Commands.MarkBookingCompleted;
using Fixy.Application.Features.Bookings.Commands.RejectBookingPriceChange;
using Fixy.Application.Features.Bookings.Commands.RequestBookingPriceChange;
using Fixy.Application.Features.Bookings.Queries.GetBookingById;
using Fixy.Application.Features.Bookings.Queries.GetBookingsForCustomer;
using Fixy.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fixy.Api.Controllers;

[Authorize]
public class BookingsController : AppControllerBase
{
<<<<<<< HEAD
    [RedisCache(60)]
=======
    //[RedisCache(60)]
>>>>>>> feature/MFA
    [Authorize(Roles = Roles.Customer)]
    [HttpGet(Router.BookingRouting.CustomerPaginatedList)]
    public async Task<IActionResult> GetBookingsForCustomer([FromQuery] GetBookingsForCustomerQuery query)
    {
        return ToActionResult(await Mediator.Send(query));
    }

<<<<<<< HEAD
    [RedisCache(60)]
=======
    //[RedisCache(60)]
>>>>>>> feature/MFA
    [HttpGet(Router.BookingRouting.GetById)]
    public async Task<IActionResult> GetBookingById([FromRoute] Guid Id)
    {
        return ToActionResult(await Mediator.Send(new GetBookingByIdQuery(Id)));
    }

    [Authorize(Roles = Roles.Technician)]
    [HttpPost(Router.BookingRouting.RequestPriceChange)]
    public async Task<IActionResult> RequestBookingPriceChange([FromBody] RequestBookingPriceChangeCommand command)
    {
        return ToActionResult(await Mediator.Send(command));
    }

    [Authorize(Roles = Roles.Customer)]
    [HttpPost(Router.BookingRouting.ApprovePriceChange)]
    public async Task<IActionResult> ApproveBookingPriceChange([FromRoute] Guid BookingId)
    {
        return ToActionResult(await Mediator.Send(new ApproveBookingPriceChangeCommand(BookingId)));
    }

    [Authorize(Roles = Roles.Customer)]
    [HttpPost(Router.BookingRouting.RejectPriceChange)]
    public async Task<IActionResult> RejectBookingPriceChange([FromRoute] Guid BookingId)
    {
        return ToActionResult(await Mediator.Send(new RejectBookingPriceChangeCommand(BookingId)));
    }

    [Authorize(Roles = Roles.Technician)]
    [HttpPost(Router.BookingRouting.MarkBookingCompleted)]
    public async Task<IActionResult> MarkBookingCompleted([FromRoute] Guid BookingId)
    {
        return ToActionResult(await Mediator.Send(new MarkBookingCompletedCommand(BookingId)));
    }

    [Authorize(Roles = Roles.Customer)]
    [HttpPost(Router.BookingRouting.ConfirmBookingCompletion)]
    public async Task<IActionResult> ConfirmBookingCompletion([FromRoute] Guid BookingId)
    {
        return ToActionResult(await Mediator.Send(new ConfirmBookingCompletionCommand(BookingId)));
    }
}
