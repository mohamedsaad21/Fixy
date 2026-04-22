using Fixy.Domain.Common;

namespace Fixy.Domain.Entities;

public class ServiceCategory : GeneralLocalizableEntity
{
    public ServiceCategory()
    {
        ServiceRequestCategories = new HashSet<ServiceRequestCategories>();
        ServiceRequests = new HashSet<ServiceRequest>();
        Technicians = new HashSet<Technician>();
    }
    public string NameEn { get; set; }
    public string NameAr { get; set; }
    public string Description { get; set; }
    public virtual ICollection<ServiceRequestCategories> ServiceRequestCategories { get; set; }
    public virtual ICollection<ServiceRequest> ServiceRequests { get; set; }
    public virtual ICollection<Technician> Technicians { get; set; }
}
