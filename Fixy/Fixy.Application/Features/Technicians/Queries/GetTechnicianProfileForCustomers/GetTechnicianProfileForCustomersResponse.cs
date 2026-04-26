namespace Fixy.Application.Features.Technicians.Queries.GetTechnicianProfileForCustomers;

public class GetTechnicianProfileForCustomersResponse
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string Bio { get; set; }
    public int YearsOfExperience { get; set; }
    public string ServiceCategoryName { get; set; }
    public string ServiceArea { get; set; }
    public int? TotalCompletedJobs { get; set; }
    public int? ComplaintsCount { get; set; }
    public int? ResponseTime { get; set; }
    public double? CancellationRate { get; set; }
    public double? AverageRating { get; set; }
}
