using Fixy.Api.Base;
using Fixy.Api.Contracts.Routing;
using Fixy.Application.Features.PriceOffers.Commands.AcceptPriceOffer;
using Fixy.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fixy.Api.Controllers;

public class PriceOffersController : AppControllerBase
{
    [Authorize(Roles = Roles.Customer)]
    [HttpPost(Router.PriceOfferRouting.AcceptPriceOffer)]
    public async Task<IActionResult> AcceptPriceOffer([FromRoute] Guid PriceOfferId)
    {
        return ToActionResult(await Mediator.Send(new AcceptPriceOfferCommand(PriceOfferId)));
    }
}
