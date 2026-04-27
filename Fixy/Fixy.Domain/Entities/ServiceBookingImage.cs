namespace Fixy.Domain.Entities;

public class ServiceBookingImage : BaseEntity
{
    public string ImageUrl { get; set; }
    public string ImagePublicId { get; set; }
    public Guid ServiceBookingId { get; set; }
    public ServiceBooking ServiceBooking { get; set; }
}
