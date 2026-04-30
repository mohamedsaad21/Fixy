using Fixy.Domain.Entities.Feedback;
using Fixy.Domain.Entities.Identity;
using Fixy.Domain.Enums;

namespace Fixy.Domain.Entities;

public class Customer : ApplicationUser
{
    public Customer()
    {
        ServiceRequests = new HashSet<ServiceRequest>();
        CustomerFeedbacks = new HashSet<CustomerFeedback>();
        TechnicianFeedbacks = new HashSet<TechnicianFeedback>();
    }
    public string? StripeCustomerId { get; set; }
    public CustomerStatus Status { get; set; } = CustomerStatus.Active;
    public ICollection<ServiceRequest> ServiceRequests { get; set; }
    public ICollection<CustomerFeedback> CustomerFeedbacks { get; set; }
    public ICollection<TechnicianFeedback> TechnicianFeedbacks { get; set; }
}
