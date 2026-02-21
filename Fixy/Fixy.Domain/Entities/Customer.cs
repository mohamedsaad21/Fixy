using Fixy.Domain.Entities.Feedback;
using Fixy.Domain.Entities.Identity;

namespace Fixy.Domain.Entities;

public class Customer : ApplicationUser
{
    public Customer()
    {
        ServiceRequests = new HashSet<ServiceRequest>();
        CustomerFeedbacks = new HashSet<CustomerFeedback>();
        TechnicianFeedbacks = new HashSet<TechnicianFeedback>();
    }
    public string? ProfilePictureUrl { get; set; }
    public string? ProfilePicturePublicId { get; set; }
    public string? StripeCustomerId { get; set; }
    public virtual ICollection<ServiceRequest> ServiceRequests { get; set; }
    public virtual ICollection<CustomerFeedback> CustomerFeedbacks { get; set; }
    public virtual ICollection<TechnicianFeedback> TechnicianFeedbacks { get; set; }
}
