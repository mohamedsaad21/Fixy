using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Bookings.Queries.GetBookingById;

public record GetBookingByIdQuery(Guid Id) : IRequest<Result<GetBookingByIdDto>>;
