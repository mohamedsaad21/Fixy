using Fixy.Domain.Enums;

namespace Fixy.Domain.Entities;

public class ServiceRequest : DatedEntity
{
    public ServiceRequest()
    {
        ServiceRequestCategories = new HashSet<ServiceRequestCategories>();
        ServiceCategories = new HashSet<ServiceCategory>();
        ServiceRequestImages = new HashSet<ServiceRequestImage>();
        ServiceBookings = new HashSet<ServiceBooking>();
        PriceOffers = new HashSet<PriceOffer>();
    }
    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; }
    public string Description { get; set; }
    public DateTime ScheduledDateTime { get; set; }
    public Address Address { get; set; }
    public ServiceRequestStatus Status { get; set; } = ServiceRequestStatus.Pending;
    public ICollection<ServiceRequestCategories> ServiceRequestCategories { get; set; }
    public ICollection<ServiceCategory> ServiceCategories { get; set; }
    public ICollection<ServiceRequestImage> ServiceRequestImages { get; set; }
    public ICollection<ServiceBooking> ServiceBookings { get; set; }
    public ICollection<PriceOffer> PriceOffers { get; set; }
}
