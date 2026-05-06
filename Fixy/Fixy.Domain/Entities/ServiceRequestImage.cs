namespace Fixy.Domain.Entities;

public class ServiceRequestImage : BaseEntity
{
    public string ImageUrl { get; set; }
    public Guid  ServiceRequestId { get; set; }
    public ServiceRequest ServiceRequest { get; set; }
}
