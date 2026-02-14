using System.ComponentModel.DataAnnotations;

namespace Fixy.Domain.Entities;

public class TechnicianStripeAccount : DatedEntity
{
    public Guid TechnicianId { get; set; }
    public string StripeAccountId { get; set; } = string.Empty;
    public string OnboardingStatus { get; set; } = "pending"; // pending, completed
    public bool ChargesEnabled { get; set; } = false;
    public bool PayoutsEnabled { get; set; } = false;
    public Technician Technician { get; set; } = null!;
}
