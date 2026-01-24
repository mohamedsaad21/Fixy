namespace Fixy.Domain.Entities;

public class ServiceRequestImage : BaseEntity
{
    public string ImageUrl { get; set; }
    public string ImagePublicId { get; set; }
    public Guid  ServiceRequestId { get; set; }
    public virtual ServiceRequest ServiceRequest { get; set; }
}
