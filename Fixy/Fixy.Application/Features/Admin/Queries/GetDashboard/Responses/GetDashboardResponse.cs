namespace Fixy.Application.Features.Admin.Queries.GetDashboard.Responses;

public class GetDashboardResponse
{
    public int TotalUsers { get; set; }
    public int TotalCustomers { get; set; }
    public int TotalTechnicians { get; set; }
    public int ActiveBookings { get; set; }
    public int CancelledBookings { get; set; }
    public int CompletedBookings { get; set; }
    public decimal Revenue { get; set; }
    public List<DailyBookingsResponse> BookingsPerDay { get; set; }
    public double CancellationRate { get; set; }
    public List<TopTechniciansResponse> TopTechnicians { get; set; }
}
