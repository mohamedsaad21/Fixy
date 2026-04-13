using Fixy.Application.Bases;
using Fixy.Application.Mapping.Bookings.Queries;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Bookings.Queries.GetBookingById;

public class GetBookingByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetBookingByIdQuery, Result<GetBookingByIdDto>>
{
    public async Task<Result<GetBookingByIdDto>> Handle(GetBookingByIdQuery request, CancellationToken cancellationToken)
    {
        var booking = await unitOfWork.Bookings.GetTableNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id);

        if (booking == null)
            return Errors.BookingNotFound;

        var result = booking.ToBookingDto();
        return result;
    }
}
