namespace Fixy.Application.Features.Technicians.Queries.GetTechnicianById;

public class GetTechnicianByIdResponse
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string NationalIdCardImageUrl { get; set; }
    public string Bio { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string NationalId { get; set; }
    public int YearsOfExperience { get; set; }
    public string ServiceCategoryName { get; set; }
    public string ServiceArea { get; set; }
    public bool IsTwoFactorEmailEnabled { get; set; }
    public int? TotalCompletedJobs { get; set; }
    public int? ComplaintsCount { get; set; }
    public int? ResponseTime { get; set; }
    public double? CancellationRate { get; set; }
    public double? AverageRating { get; set; }
    public string Status { get; set; }
    public bool IsActive { get; set; }
}
