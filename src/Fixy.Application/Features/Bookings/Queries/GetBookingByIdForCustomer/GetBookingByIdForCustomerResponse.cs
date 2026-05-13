using Fixy.Application.Common.DTOs.ServiceRequest;

namespace Fixy.Application.Features.Bookings.Queries.GetBookingByIdForCustomer;

public class GetBookingByIdForCustomerResponse
{
    public Guid Id { get; set; }
    public string Status { get; set; }
    public decimal AgreedPrice { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public Guid TechnicianId { get; set; }
    public string TechnicianName { get; set; }
    public string TechnicianProfilePicture { get; set; }
    public string Description { get; set; }
    public Guid ConversationId { get; set; }
    public AddressDto Address { get; set; }
    public List<ImageDto> Images { get; set; }
}