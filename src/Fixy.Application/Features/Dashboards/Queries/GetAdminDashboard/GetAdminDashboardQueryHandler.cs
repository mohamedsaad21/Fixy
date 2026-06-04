using Fixy.Application.Bases;
using Fixy.Application.Features.Dashboards.Queries.GetAdminDashboard.Responses;
using Fixy.Domain.Entities.Identity;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fixy.Application.Features.Dashboards.Queries.GetAdminDashboard;

public sealed class GetAdminDashboardQueryHandler(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, ILogger<GetAdminDashboardQueryHandler> logger) : IRequestHandler<GetAdminDashboardQuery, Result<GetDashboardResponse>>
{
    public async Task<Result<GetDashboardResponse>> Handle(GetAdminDashboardQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Admin dashboard data requested.");
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
            .CountAsync(x => x.Status == ServiceBookingStatus.FullCompleted, cancellationToken);

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
            .Where(x => x.Status == ServiceBookingStatus.FullCompleted)
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

        logger.LogInformation("Admin dashboard data assembled successfully. TotalUsers: {TotalUsers}, TotalCustomers: {TotalCustomers}, TotalTechnicians: {TotalTechnicians}, TotalBookings: {TotalBookings}, ActiveBookings: {ActiveBookings}, CompletedBookings: {CompletedBookings}, CancelledBookings: {CancelledBookings}, CancellationRate: {CancellationRate:F2}, TopTechniciansResolved: {TopTechniciansResolved}",
            totalUsers, totalCustomers, totalTechnicians, totalBookings,
            activeBookings, completedBookings, cancelledBookings,
            cancellationRate, topTechnicians.Count);

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