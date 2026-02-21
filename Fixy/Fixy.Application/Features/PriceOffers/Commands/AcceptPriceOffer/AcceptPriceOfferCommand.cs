using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.PriceOffers.Commands.AcceptPriceOffer;

public sealed record AcceptPriceOfferCommand(Guid PriceOfferId) : IRequest<Result>;