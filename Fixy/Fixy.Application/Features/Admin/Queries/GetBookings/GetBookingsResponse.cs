namespace Fixy.Application.Features.Admin.Queries.GetBookings;

public class GetBookingsResponse
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid TechnicianId { get; set; }
    public string CustomerName { get; set; }
    public string TechnicianName { get; set; }
    public string CustomerUserName { get; set; }
    public string TechnicianUserName { get; set; }
    public decimal Price { get; set; }
    public string Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
}
