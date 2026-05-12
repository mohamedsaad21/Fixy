using Fixy.Domain.Enums;

namespace Fixy.Domain.Entities.Feedback;

public class CustomerFeedback : BaseEntity
{
    public Guid ServiceBookingId { get; set; }
    public ServiceBooking ServiceBooking { get; set; }
    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; }
    public Guid TechnicianId { get; set; }
    public Technician Technician { get; set; }
    public int JobDurationMinutes { get; set; }
    public int ResponseTimeMinutes { get; set; }
    public int PunctualityRating { get; set; }
    public int ProfessionalismRating { get; set; }
    public int CommunicationRating { get; set; }
    public int HandlingQuality { get; set; }
    public int CostSatisfaction { get; set; }
    public WeatherCondition WeatherCondition { get; set; }
    public DateTimeOffset SubmittedAt { get; set; }
    public string? TextComplain { get; set; }
}
