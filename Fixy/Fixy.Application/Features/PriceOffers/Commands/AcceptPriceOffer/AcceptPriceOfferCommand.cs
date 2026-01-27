using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.PriceOffers.Commands.AcceptPriceOffer;

public record AcceptPriceOfferCommand(Guid PriceOfferId) : IRequest<Result>;