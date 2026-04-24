namespace Fixy.Application.Features.ServiceRequests.Queries.GetServiceRequestById.Responses;

public class PriceOfferDto
{
    public Guid Id { get; set; }
    public string TechnicianUserName { get; set; }
    public double? AverageRating { get; set; }
    public decimal Price { get; set; }
    public double DistanceKm { get; set; }
    public DateTime CreatedAt { get; set; }
}