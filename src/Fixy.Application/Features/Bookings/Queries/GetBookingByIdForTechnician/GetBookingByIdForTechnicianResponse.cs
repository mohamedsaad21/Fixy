using Fixy.Application.Common.DTOs.ServiceRequest;

namespace Fixy.Application.Features.Bookings.Queries.GetBookingByIdForTechnician;

public class GetBookingByIdForTechnicianResponse
{
    public Guid Id { get; set; }
    public string Status { get; set; }
    public decimal AgreedPrice { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; }
    public string CustomerProfilePicture { get; set; }
    public string Description { get; set; }
    public AddressDto Address { get; set; }
    public List<ImageDto> Images { get; set; }
}
