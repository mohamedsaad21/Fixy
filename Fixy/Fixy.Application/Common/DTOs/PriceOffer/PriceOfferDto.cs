namespace Fixy.Application.Common.DTOs.PriceOffer;

public class PriceOfferDto
{
    public Guid Id { get; set; }
    public Guid TechnicianId { get; set; }
    public string TechnicianFullName { get; set; }
    public string TechnicianUserName { get; set; }
    public string TechnicianCategory { get; set; }
    public double? AverageRating { get; set; }
    public decimal Price { get; set; }
    public double DistanceKm { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}