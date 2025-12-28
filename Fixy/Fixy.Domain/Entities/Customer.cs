using Fixy.Domain.Entities.Identity;

namespace Fixy.Domain.Entities;

public class Customer : ApplicationUser
{
    public virtual ICollection<ServiceRequest> ServiceRequests { get; set; }
}
