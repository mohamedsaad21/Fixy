using AutoMapper;
using Fixy.Application.Bases;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Admin.Queries.GetBookingById;

public sealed class GetBookingByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetBookingByIdQuery, Result<GetBookingByIdResponse>>
{
    public async Task<Result<GetBookingByIdResponse>> Handle(GetBookingByIdQuery request, CancellationToken cancellationToken)
    {
        var booking = await unitOfWork.Bookings.GetTableNoTracking()
            .Include(x => x.ServiceRequest).ThenInclude(x => x.Customer)
            .Include(x => x.Technician).ThenInclude(x => x.ServiceCategory)
            .FirstOrDefaultAsync(x => x.Id == request.Id);

        if (booking == null)
            return Errors.BookingNotFound;

        var bookingResponse = mapper.Map<GetBookingByIdResponse>(booking);
        return bookingResponse;
    }
}
