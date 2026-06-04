namespace Fixy.Application.Features.Bookings.Queries.GetBookingsForTechnician;

public class GetBookingsForTechnicianResponse
{
    public Guid Id { get; set; }
    public string Description { get; set; }
    public string Status { get; set; }
    public decimal AgreedPrice { get; set; }
    public DateTimeOffset ScheduledDateTime { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
