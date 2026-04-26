using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Mapping.Bookings.Queries;
using Fixy.Domain.Entities;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Bookings.Queries.GetBookingByIdForCustomer;

public class GetBookingByIdForCustomerQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService) : IRequestHandler<GetBookingByIdForCustomerQuery, Result<GetBookingByIdForCustomerResponse>>
{
    public async Task<Result<GetBookingByIdForCustomerResponse>> Handle(GetBookingByIdForCustomerQuery request, CancellationToken cancellationToken)
    {
        var currentUser = await currentUserService.GetCurrentUserAsync();

        if (currentUser is not Customer customer)
            return Errors.Unauthorized;

        var booking = await unitOfWork.Bookings.GetTableNoTracking().Include(x => x.ServiceRequest)
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.ServiceRequest.CustomerId == customer.Id);

        if (booking == null)
            return Errors.BookingNotFound;

        var result = booking.ToGetBookingByIdForCustomerResponse();
        return result;
    }
}
