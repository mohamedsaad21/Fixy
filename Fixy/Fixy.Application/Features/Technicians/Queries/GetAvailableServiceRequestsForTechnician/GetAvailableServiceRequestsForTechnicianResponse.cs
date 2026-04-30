using Fixy.Application.Common.DTOs;

namespace Fixy.Application.Features.Technicians.Queries.GetAvailableServiceRequestsForTechnician;

public class GetAvailableServiceRequestsForTechnicianResponse
{
    public Guid Id { get; set; }
    public string Description { get; set; }
    public DateTime ScheduledDateTime { get; set; }
    public AddressDto Address { get; set; }
    public string Status { get; set; }
    public List<string> ServiceCategories { get; set; }
    public List<string> ImageUrls { get; set; }
    public int PriceOffersCount { get; set; }
    public double DistanceKm { get; set; }
}
