namespace Fixy.Domain.Entities;

public class ServiceCategory : DatedEntity
{
    public ServiceCategory()
    {
        ServiceRequestCategories = new HashSet<ServiceRequestCategories>();
        ServiceRequests = new HashSet<ServiceRequest>();
    }
    public string Name { get; set; }
    public string Description { get; set; }
    public virtual ICollection<ServiceRequestCategories> ServiceRequestCategories { get; set; }
    public virtual ICollection<ServiceRequest> ServiceRequests { get; set; }
    public virtual ICollection<Technician> Technicians { get; set; }
}
