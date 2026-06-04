using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fixy.Application.Features.Dashboards.Queries.GetTechnicianDashboard;

public sealed class GetTechnicianDashboardQueryHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork, ILogger<GetTechnicianDashboardQueryHandler> logger) : IRequestHandler<GetTechnicianDashboardQuery, Result<GetTechnicianDashboardResponse>>
{
    public async Task<Result<GetTechnicianDashboardResponse>> Handle(GetTechnicianDashboardQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Technician dashboard data requested.");
        
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

        logger.LogInformation("Technician dashboard data assembled successfully. TechnicianId: {TechnicianId}, InProgressBookings: {InProgressBookings}, CompletedBookings: {CompletedBookings}, CancelledBookings: {CancelledBookings}, CancellationRate: {CancellationRate:F2}, AverageRating: {AverageRating:F2}",
            technician.Id, inProgressBookingsCount, completedBookingsCount,
            cancelledBookingsCount, technician.CancellationRate, technician.AverageRating);

        return result;
    }
}
