using Fixy.Domain.Entities.Identity;
using Fixy.Domain.Enums;
using System.Net.NetworkInformation;

namespace Fixy.Domain.Entities;

public class Dispute : BaseEntity
{
    public Guid ServiceBookingId { get; set; }
    public ServiceBooking ServiceBooking { get; set; }
    public Guid RaiserId { get; set; }
    public ApplicationUser Raiser { get; set; }
    public string Reason { get; set; }
    public string DesiredResolution { get; set; }
    public string Status { get; set; }
    public string ResolutionOutcome { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ResolvedAt { get; set; }
    public string StripeDisputeId { get; set; }
    public Guid ResolverId { get; set; }
    public ApplicationUser Resolver { get; set; }
}