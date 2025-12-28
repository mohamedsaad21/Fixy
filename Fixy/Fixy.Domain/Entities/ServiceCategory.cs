namespace Fixy.Domain.Entities;

public class ServiceCategory : DatedEntity
{
    public ServiceCategory()
    {
        ServiceRequestCategories = new HashSet<ServiceRequestCategories>();
    }
    public string Name { get; set; }
    public string Description { get; set; }
    public virtual ICollection<ServiceRequestCategories> ServiceRequestCategories { get; set; }
}
