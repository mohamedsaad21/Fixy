namespace Fixy.Application.Features.Admin.Queries.GetDashboard.Responses;

public class TopTechniciansResponse
{
    public Guid TechnicianId { get; set; }
    public string Name { get; set; }
    public int CompletedBookings { get; set; }
}
