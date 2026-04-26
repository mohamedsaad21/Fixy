using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Bookings.Queries.GetBookingByIdForCustomer;

public record GetBookingByIdForCustomerQuery(Guid Id) : IRequest<Result<GetBookingByIdForCustomerResponse>>;
