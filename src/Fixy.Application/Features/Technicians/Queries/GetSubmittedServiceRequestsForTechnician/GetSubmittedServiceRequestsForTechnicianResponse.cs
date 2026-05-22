using Fixy.Application.Common.DTOs.ServiceRequest;

namespace Fixy.Application.Features.Technicians.Queries.GetSubmittedServiceRequestsForTechnician;

public class GetSubmittedServiceRequestsForTechnicianResponse
{
    public Guid Id { get; set; }
    public string Description { get; set; }
    public DateTimeOffset ScheduledDateTime { get; set; }
    public AddressDto Address { get; set; }
    public string Status { get; set; }
    public List<string> ServiceCategories { get; set; }
    public List<string> ImageUrls { get; set; }
    public decimal PriceOffer { get; set; }
    public int PriceOffersCount { get; set; }
}
