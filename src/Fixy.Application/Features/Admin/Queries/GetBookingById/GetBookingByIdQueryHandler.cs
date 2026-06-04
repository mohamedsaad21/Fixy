using AutoMapper;
using Fixy.Application.Bases;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fixy.Application.Features.Admin.Queries.GetBookingById;

public sealed class GetBookingByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetBookingByIdQueryHandler> logger) : IRequestHandler<GetBookingByIdQuery, Result<GetBookingByIdResponse>>
{
    public async Task<Result<GetBookingByIdResponse>> Handle(GetBookingByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching booking by ID. BookingId: {BookingId}", request.Id);

        var booking = await unitOfWork.Bookings.GetTableNoTracking()
            .Include(x => x.ServiceRequest).ThenInclude(x => x.Customer)
            .Include(x => x.Technician).ThenInclude(x => x.ServiceCategory)
            .FirstOrDefaultAsync(x => x.Id == request.Id);

        if (booking == null)
        {
            logger.LogWarning("Booking not found. BookingId: {BookingId}", request.Id);
            return Errors.BookingNotFound;
        }

        var bookingResponse = mapper.Map<GetBookingByIdResponse>(booking);
        logger.LogInformation("Booking fetched successfully. BookingId: {BookingId}, Status: {Status}, CustomerId: {CustomerId}, TechnicianId: {TechnicianId}", booking.Id, booking.Status, booking.ServiceRequest.Customer.Id, booking.Technician.Id);
        return bookingResponse;
    }
}
