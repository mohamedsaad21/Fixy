namespace Fixy.Application.Features.Admin.Queries.GetCustomers;

public class GetCustomersResponse
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Status { get; set; }
    public int? TotalBookings { get; set; }
    public int? CompletedBookings { get; set; }
    public int? CancelledBookings { get; set; }
    public double? CancellationRate { get; set; }
}

