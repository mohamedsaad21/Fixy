using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.PriceOffers.Commands.CreatePriceOffer;

public record CreatePriceOfferCommand
    (
            Guid ServiceRequestId,
            decimal Price
    ) : IRequest<Result>;