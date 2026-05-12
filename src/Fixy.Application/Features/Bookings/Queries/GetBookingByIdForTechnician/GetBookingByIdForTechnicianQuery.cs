using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Bookings.Queries.GetBookingByIdForTechnician;

public sealed record GetBookingByIdForTechnicianQuery(Guid Id) : IRequest<Result<GetBookingByIdForTechnicianResponse>>;