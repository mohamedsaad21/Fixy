namespace Fixy.Application.Features.Technicians.Queries.GetCustomerProfileForTechnicians;

public class GetCustomerProfileForTechniciansResponse
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string Bio { get; set; }
    public int? TotalBookings { get; set; }
    public int? CompletedBookings { get; set; }
    public int? CancelledBookings { get; set; }
    public double? CancellationRate { get; set; }
}
