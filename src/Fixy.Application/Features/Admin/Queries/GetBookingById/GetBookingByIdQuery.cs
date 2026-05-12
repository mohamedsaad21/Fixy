using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Admin.Queries.GetBookingById;

public sealed record GetBookingByIdQuery(Guid Id) : IRequest<Result<GetBookingByIdResponse>>;
