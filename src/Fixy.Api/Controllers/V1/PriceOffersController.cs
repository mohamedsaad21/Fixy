using Fixy.Api.Attributes;
﻿using Asp.Versioning;
using Fixy.Api.Contracts.Routing;
using Fixy.Api.Controllers.Common;
using Fixy.Application.Features.PriceOffers.Commands.AcceptPriceOffer;
using Fixy.Application.Features.PriceOffers.Commands.CreatePriceOffer;
using Fixy.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fixy.Api.Controllers.V1;

[ApiVersion("1.0")]
public class PriceOffersController : AppControllerBase
{
    [Authorize(Roles = Roles.Technician)]
    //[RequireActiveTechnician]
    //[RequireTechnicianFeedback]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPost(Router.PriceOfferRouting.CreatePriceOffer)]
    public async Task<IActionResult> CreatePriceOffer([FromBody] CreatePriceOfferCommand command)
    {
        return ToActionResult(await Mediator.Send(command));
    }

    [Authorize(Roles = Roles.Customer)]
    [RequireActiveCustomer]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPost(Router.PriceOfferRouting.AcceptPriceOffer)]
    public async Task<IActionResult> AcceptPriceOffer([FromRoute] Guid PriceOfferId)
    {
        return ToActionResult(await Mediator.Send(new AcceptPriceOfferCommand(PriceOfferId)));
    }
}
