using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Dashboards.Queries.GetTechnicianDashboard;

public sealed class GetTechnicianDashboardQueryHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork) : IRequestHandler<GetTechnicianDashboardQuery, Result<GetTechnicianDashboardResponse>>
{
    public async Task<Result<GetTechnicianDashboardResponse>> Handle(GetTechnicianDashboardQuery request, CancellationToken cancellationToken)
    {
        var currentTechnicianId = await currentUserService.GetCurrentUserId();

        var technician = await unitOfWork.Technicians.GetTableNoTracking().FirstOrDefaultAsync(x => x.Id == currentTechnicianId);

        if (technician == null)
            return Errors.TechnicianNotFound;

        var bookingsQuery = unitOfWork.Bookings.GetTableNoTracking().Where(x => x.TechnicianId == technician.Id).AsQueryable();

        var inProgressBookingsCount =
            await bookingsQuery.CountAsync(x => x.Status == ServiceBookingStatus.InProgress);

        var completedBookingsCount =
            await bookingsQuery.CountAsync(x => x.Status == ServiceBookingStatus.TechnicianCompleted);

        var cancelledBookingsCount =
            await bookingsQuery.CountAsync(x => x.Status == ServiceBookingStatus.CancelledByTechnician);

        var result = new GetTechnicianDashboardResponse
        {
            InProgressBookingsCount = inProgressBookingsCount,
            CompletedBookingsCount = completedBookingsCount,
            CancelledBookingsCount = cancelledBookingsCount,
            CancellationRate = technician.CancellationRate
        };
        return result;
    }
}
