namespace Fixy.Application.Features.Admin.Queries.GetTechnicians;

public class GetTechniciansResponse
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Status { get; set; }
    public int? TotalCompletedJobs { get; set; }
    public double? CancellationRate { get; set; }
    public double ?AverageRating { get; set; }
}
