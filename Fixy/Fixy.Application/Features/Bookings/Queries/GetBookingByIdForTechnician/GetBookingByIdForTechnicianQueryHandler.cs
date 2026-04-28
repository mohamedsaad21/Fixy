using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Mapping.Bookings.Queries;
using Fixy.Application.Resources;
using Fixy.Domain.Entities;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Features.Bookings.Queries.GetBookingByIdForTechnician;

public sealed class GetBookingByIdForTechnicianQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IStringLocalizer<SharedResources> localizer) : IRequestHandler<GetBookingByIdForTechnicianQuery, Result<GetBookingByIdForTechnicianResponse>>
{
    public async Task<Result<GetBookingByIdForTechnicianResponse>> Handle(GetBookingByIdForTechnicianQuery request, CancellationToken cancellationToken)
    {
        var currentUser = await currentUserService.GetCurrentUserAsync();

        if (currentUser is not Technician technician)
            return Errors.Unauthorized;

        var booking = await unitOfWork.Bookings.GetTableNoTracking().Include(x => x.ServiceRequest).ThenInclude(x => x.Customer)
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.TechnicianId == technician.Id);

        if (booking == null)
            return Errors.BookingNotFound;

        var result = booking.ToGetBookingByIdForTechnicianResponse(localizer);
        return result;
    }
}
