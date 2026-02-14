using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Technicians.Queries.GetStripeStatus;

public record GetStripeStatusQuery() : IRequest<Result<GetStripeStatusResponse>>;
