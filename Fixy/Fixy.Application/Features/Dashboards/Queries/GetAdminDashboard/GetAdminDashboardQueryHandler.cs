using Fixy.Application.Bases;
using Fixy.Application.Features.Dashboards.Queries.GetAdminDashboard.Responses;
using Fixy.Domain.Entities.Identity;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Dashboards.Queries.GetAdminDashboard;

public sealed class GetAdminDashboardQueryHandler(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager) : IRequestHandler<GetAdminDashboardQuery, Result<GetDashboardResponse>>
{
    public async Task<Result<GetDashboardResponse>> Handle(GetAdminDashboardQuery request, CancellationToken cancellationToken)
    {
        // 🔢 KPIs
        var totalUsers = await userManager.Users.CountAsync(cancellationToken);

        var totalCustomers = await unitOfWork.Customers.GetTableNoTracking().CountAsync();
        var totalTechnicians = await unitOfWork.Technicians.GetTableNoTracking().CountAsync();


        var bookingsQuery = unitOfWork.Bookings.GetTableNoTracking();

        var activeBookings = await bookingsQuery
            .CountAsync(x => x.Status == ServiceBookingStatus.InProgress, cancellationToken);

        var cancelledBookings = await bookingsQuery
            .CountAsync(x => x.Status == ServiceBookingStatus.CancelledByCustomer, cancellationToken);

        var completedBookings = await bookingsQuery
            .CountAsync(x => x.Status == ServiceBookingStatus.Completed, cancellationToken);

        var totalBookings = await bookingsQuery.CountAsync(cancellationToken);

        // 📈 Bookings per day (last 7 days)
        var fromDate = DateTimeOffset.UtcNow.AddDays(-7);

        var bookingsPerDay = await bookingsQuery
            .Where(x => x.CreatedAt >= fromDate)
            .GroupBy(x => x.CreatedAt)
            .Select(g => new DailyBookingsResponse
            {
                Date = g.Key,
                Count = g.Count()
            })
            .OrderBy(x => x.Date)
            .ToListAsync(cancellationToken);

        // 📊 Cancellation Rate
        double cancellationRate = totalBookings == 0
            ? 0
            : (double)cancelledBookings / totalBookings * 100;

        // 🧑‍🔧 Top Technicians
        var topTechniciansRaw = await bookingsQuery
            .Where(x => x.Status == ServiceBookingStatus.Completed)
            .GroupBy(x => x.TechnicianId)
            .Select(g => new
            {
                TechnicianId = g.Key,
                CompletedBookings = g.Count()
            })
            .OrderByDescending(x => x.CompletedBookings)
            .Take(5)
            .ToListAsync(cancellationToken);

        var technicianIds = topTechniciansRaw
            .Select(x => x.TechnicianId)
            .ToList();

        var users = await userManager.Users
            .Where(x => technicianIds.Contains(x.Id))
            .ToListAsync(cancellationToken);

        var topTechnicians = topTechniciansRaw
            .Select(t => new TopTechniciansResponse
            {
                TechnicianId = t.TechnicianId,
                CompletedBookings = t.CompletedBookings,
                Name = users.FirstOrDefault(u => u.Id == t.TechnicianId)?.UserName ?? "Unknown"
            })
            .ToList();

        // 💰 Revenue (V1 = 0)
        decimal revenue = 0; // implement later when payment ready


        return new GetDashboardResponse
        {
            TotalUsers = totalUsers,
            TotalCustomers = totalCustomers,
            TotalTechnicians = totalTechnicians,

            ActiveBookings = activeBookings,
            CancelledBookings = cancelledBookings,
            CompletedBookings = completedBookings,

            Revenue = revenue,

            BookingsPerDay = bookingsPerDay,
            CancellationRate = cancellationRate,
            TopTechnicians = topTechnicians
        };
    }
}