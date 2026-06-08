using Fixy.Domain.Enums;

namespace Fixy.Application.Features.Admin.Queries.GetDisputes;

public class DisputeDto
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public string RaiserName { get; set; }
    public string Reason { get; set; }
    public string? DesiredResolution { get; set; }
    public DisputeStatus Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
