namespace Fixy.Domain.Entities;

public class BlockedServiceRequest : BaseEntity
{
    public Guid TechnicianId { get; set; }
    public Technician Technician { get; set; }
    public Guid ServiceRequestId { get; set; }
    public ServiceRequest ServiceRequest { get; set; }
}
