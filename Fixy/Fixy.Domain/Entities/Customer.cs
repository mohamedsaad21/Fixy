using Fixy.Domain.Entities.Identity;

namespace Fixy.Domain.Entities;

public class Customer : ApplicationUser
{
    public string? ProfilePictureUrl { get; set; }
    public string? ProfilePicturePublicId { get; set; }
    public string? StripeCustomerId { get; set; }
    public virtual ICollection<ServiceRequest> ServiceRequests { get; set; }
}
