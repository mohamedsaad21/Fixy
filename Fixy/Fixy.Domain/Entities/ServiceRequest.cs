using Fixy.Domain.Enums;

namespace Fixy.Domain.Entities;

public class ServiceRequest : DatedEntity
{
    public ServiceRequest()
    {
        ServiceRequestCategories = new HashSet<ServiceRequestCategories>();
        ServiceCategories = new HashSet<ServiceCategory>();
        ServiceRequestImages = new HashSet<ServiceRequestImage>();
    }
    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; }
    public string Description { get; set; }
    public DateTime ScheduledDateTime { get; set; }
    public Address Address { get; set; }
    public ServiceRequestStatus Status { get; set; } = ServiceRequestStatus.Pending;
    public virtual ICollection<ServiceCategory> ServiceCategories { get; set; }
    public virtual ICollection<ServiceRequestCategories> ServiceRequestCategories { get; set; }
    public virtual ICollection<ServiceRequestImage> ServiceRequestImages { get; set; }
}
