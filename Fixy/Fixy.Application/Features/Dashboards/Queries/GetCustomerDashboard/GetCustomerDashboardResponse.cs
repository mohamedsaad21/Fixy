namespace Fixy.Application.Features.Dashboards.Queries.GetCustomerDashboard;

public class GetCustomerDashboardResponse
{
    public int PendingServiceRequestsCount { get; set; }
    public int CancelledServiceRequestsCount { get; set; }
    public int InProgressBookingsCount { get; set; }
    public int CompletedBookingsCount { get; set; }
    public int CancelledBookingsCount { get; set; }
    public double? CancellationRate { get; set; }
}
