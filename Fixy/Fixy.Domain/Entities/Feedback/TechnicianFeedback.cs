namespace Fixy.Domain.Entities.Feedback;

public class TechnicianFeedback : BaseEntity
{
    public Guid ServiceBookingId { get; set; }
    public ServiceBooking ServiceBooking { get; set; }
    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; }
    public Guid TechnicianId { get; set; }
    public Technician Technician { get; set; }
    public double CustomerBehaviorRating { get; set; }
    public double ClarityOfIssue { get; set; }
    public double SafetyAndEnvironment { get; set; }
    public double CustomerPunctuality { get; set; }
    public double WorkerSatisfaction { get; set; }
    public DateTime SubmittedAt { get; set; }
}
