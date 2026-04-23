using Fixy.Api.Attributes;
using Fixy.Api.Base;
using Fixy.Api.Contracts.Routing;
using Fixy.Application.Features.Bookings.Commands.ApproveBookingPriceChange;
using Fixy.Application.Features.Bookings.Commands.CancelBookingByCustomer;
using Fixy.Application.Features.Bookings.Commands.CancelBookingByTechnician;
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
    //[RedisCache(60)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize(Roles = Roles.Customer)]
    [HttpGet(Router.BookingRouting.CustomerPaginatedList)]
    public async Task<IActionResult> GetBookingsForCustomer([FromQuery] GetBookingsForCustomerQuery query)
    {
        return ToActionResult(await Mediator.Send(query));
    }

    //[RedisCache(60)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet(Router.BookingRouting.GetById)]
    public async Task<IActionResult> GetBookingById([FromRoute] Guid Id)
    {
        return ToActionResult(await Mediator.Send(new GetBookingByIdQuery(Id)));
    }

    [Authorize(Roles = Roles.Technician)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPost(Router.BookingRouting.RequestPriceChange)]
    public async Task<IActionResult> RequestBookingPriceChange([FromBody] RequestBookingPriceChangeCommand command)
    {
        return ToActionResult(await Mediator.Send(command));
    }

    [Authorize(Roles = Roles.Customer)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPost(Router.BookingRouting.ApprovePriceChange)]
    public async Task<IActionResult> ApproveBookingPriceChange([FromRoute] Guid BookingId)
    {
        return ToActionResult(await Mediator.Send(new ApproveBookingPriceChangeCommand(BookingId)));
    }

    [Authorize(Roles = Roles.Customer)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPost(Router.BookingRouting.RejectPriceChange)]
    public async Task<IActionResult> RejectBookingPriceChange([FromRoute] Guid BookingId)
    {
        return ToActionResult(await Mediator.Send(new RejectBookingPriceChangeCommand(BookingId)));
    }

    [Authorize(Roles = Roles.Technician)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPost(Router.BookingRouting.MarkBookingCompleted)]
    public async Task<IActionResult> MarkBookingCompleted([FromForm] MarkBookingCompletedCommand command)
    {
        return ToActionResult(await Mediator.Send(command));
    }

    [Authorize(Roles = Roles.Customer)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPost(Router.BookingRouting.ConfirmBookingCompletion)]
    public async Task<IActionResult> ConfirmBookingCompletion([FromRoute] Guid BookingId)
    {
        return ToActionResult(await Mediator.Send(new ConfirmBookingCompletionCommand(BookingId)));
    }

    [Authorize(Roles = Roles.Customer)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPost(Router.BookingRouting.CancelBookingByCustomer)]
    public async Task<IActionResult> CancelBookingByCustomer([FromForm] CancelBookingByCustomerCommand command)
    {
        return ToActionResult(await Mediator.Send(command));
    }

    [Authorize(Roles = Roles.Technician)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPost(Router.BookingRouting.CancelBookingByTechnician)]
    public async Task<IActionResult> CancelBookingByTechnician([FromForm] CancelBookingByTechnicianCommand command)
    {
        return ToActionResult(await Mediator.Send(command));
    }
}
