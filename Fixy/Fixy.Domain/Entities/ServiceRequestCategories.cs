namespace Fixy.Domain.Entities;

public class ServiceRequestCategories : BaseEntity
{
    public Guid CategoryId { get; set; } 
    public ServiceCategory Category { get; set; } 
    public Guid ServiceRequestId { get; set; }
    public ServiceRequest ServiceRequest { get; set; }
}
