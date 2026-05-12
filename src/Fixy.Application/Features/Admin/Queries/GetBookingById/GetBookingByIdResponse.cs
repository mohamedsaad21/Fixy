namespace Fixy.Application.Features.Admin.Queries.GetBookingById;

public class GetBookingByIdResponse
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid TechnicianId { get; set; }
    public string CustomerName { get; set; }
    public string TechnicianName { get; set; }
    public string CustomerUserName { get; set; }
    public string TechnicianUserName { get; set; }
    public string Description { get; set; }
    public string ServiceCategory { get; set; }
    public decimal Price { get; set; }
    public string Status { get; set; }
    public string? CancellationNote { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
}
