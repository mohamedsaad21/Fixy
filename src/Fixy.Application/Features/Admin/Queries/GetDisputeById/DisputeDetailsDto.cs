using Fixy.Domain.Enums;

namespace Fixy.Application.Features.Admin.Queries.GetDisputeById;

public class DisputeDetailsDto
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public Guid TechnicianId { get; set; }
    public string TechnicianName { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; }
    public Guid RaiserId { get; set; }
    public string RaiserName { get; set; }
    public string Reason { get; set; }
    public string? DesiredResolution { get; set; }
    public DisputeStatus Status { get; set; }
    public string? ResolutionOutcome { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? ResolvedAt { get; set; }
    public string? ResolverName { get; set; }
}
