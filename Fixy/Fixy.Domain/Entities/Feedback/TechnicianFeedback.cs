namespace Fixy.Domain.Entities.Feedback;

public class TechnicianFeedback : BaseEntity
{
    public Guid ServiceBookingId { get; set; }
    public ServiceBooking ServiceBooking { get; set; }
    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; }
    public Guid TechnicianId { get; set; }
    public Technician Technician { get; set; }
    public int CustomerBehaviorRating { get; set; }
    public int ClarityOfIssue { get; set; }
    public int SafetyAndEnvironment { get; set; }
    public int CustomerPunctuality { get; set; }
    public int WorkerSatisfaction { get; set; }
    public DateTimeOffset SubmittedAt { get; set; }
}
