using AutoMapper;
using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Entities;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Bookings.Queries.GetBookingByIdForCustomer;

public class GetBookingByIdForCustomerQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IMapper mapper) : IRequestHandler<GetBookingByIdForCustomerQuery, Result<GetBookingByIdForCustomerResponse>>
{
    public async Task<Result<GetBookingByIdForCustomerResponse>> Handle(GetBookingByIdForCustomerQuery request, CancellationToken cancellationToken)
    {
        var currentCustomerId = currentUserService.GetCurrentUserId();

        var customer = await unitOfWork.Customers.GetTableNoTracking().FirstOrDefaultAsync(x => x.Id == currentCustomerId);

        if (customer == null)
            return Errors.Unauthorized;

        var booking = await unitOfWork.Bookings.GetTableNoTracking()
            .Include(x => x.ServiceRequest).ThenInclude(x => x.ServiceRequestImages).Include(x => x.Technician)
            .FirstOrDefaultAsync(x => x.Id == request.Id);

        if (booking == null)
            return Errors.BookingNotFound;

        var result = mapper.Map<GetBookingByIdForCustomerResponse>(booking);
        return result;
    }
}
