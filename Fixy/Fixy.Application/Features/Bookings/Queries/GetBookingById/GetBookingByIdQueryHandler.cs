using Fixy.Application.Bases;
using Fixy.Application.Mapping.Bookings.Queries;
using Fixy.Infrastructure.Persistence.Abstracts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Bookings.Queries.GetBookingById;

public class GetBookingByIdQueryHandler : IRequestHandler<GetBookingByIdQuery, Result<GetBookingByIdDto>>
{
    private readonly IServiceBookingRepository _serviceBookingRepository;

    public GetBookingByIdQueryHandler(IServiceBookingRepository serviceBookingRepository)
    {
        _serviceBookingRepository = serviceBookingRepository;
    }

    public async Task<Result<GetBookingByIdDto>> Handle(GetBookingByIdQuery request, CancellationToken cancellationToken)
    {
        var booking = await _serviceBookingRepository.GetTableNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id);

        if (booking == null)
            return Errors.BookingNotFound;

        var result = booking.ToBookingDto();
        return result;
    }
}
