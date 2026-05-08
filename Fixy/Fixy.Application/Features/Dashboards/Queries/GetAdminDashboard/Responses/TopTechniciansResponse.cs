namespace Fixy.Application.Features.Dashboards.Queries.GetAdminDashboard.Responses;

public class TopTechniciansResponse
{
    public Guid TechnicianId { get; set; }
    public string Name { get; set; }
    public int CompletedBookings { get; set; }
}
