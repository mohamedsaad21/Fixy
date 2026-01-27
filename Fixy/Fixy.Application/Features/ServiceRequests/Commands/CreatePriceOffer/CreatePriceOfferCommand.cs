using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.ServiceRequests.Commands.CreatePriceOffer;

public record CreatePriceOfferCommand
    (
            Guid ServiceRequestId,
            decimal MinPrice,
            decimal MaxPrice
    ) : IRequest<Result>;