namespace Fixy.Application.Features.Bookings.Queries.GetBookingByIdForCustomer;

public class GetBookingByIdForCustomerResponse
{
    public Guid Id { get; set; }
    public string Status { get; set; }
    public decimal AgreedPrice { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid TechnicianId { get; set; }
    public string TechnicianName { get; set; }
}