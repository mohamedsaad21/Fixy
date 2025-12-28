using Fixy.Domain.Enums;

namespace Fixy.Domain.Entities;

public class ServiceRequest : BaseEntity
{
    public ServiceRequest()
    {
        ServiceRequestCategories = new HashSet<ServiceRequestCategories>();
    }
    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; }
    public string Description { get; set; }
    public DateTime ScheduledDateTime { get; set; }
    public ServiceRequestStatus Status { get; set; } = ServiceRequestStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public virtual ICollection<ServiceRequestCategories> ServiceRequestCategories { get; set; }
}
