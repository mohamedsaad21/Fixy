namespace Fixy.Application.Features.Bookings.Queries.GetBookingByIdForTechnician;

public class GetBookingByIdForTechnicianResponse
{
    public Guid Id { get; set; }
    public string Status { get; set; }
    public decimal AgreedPrice { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CustomerId { get; set; }
}
