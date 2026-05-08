namespace Fixy.Application.Features.Dashboards.Queries.GetTechnicianDashboard;

public class GetTechnicianDashboardResponse
{
    public int InProgressBookingsCount { get; set; }
    public int CompletedBookingsCount { get; set; }
    public int CancelledBookingsCount { get; set; }
    public double? CancellationRate { get; set; }
}
