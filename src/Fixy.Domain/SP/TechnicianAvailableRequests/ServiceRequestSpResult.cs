namespace Fixy.Domain.SP.TechnicianAvailableRequests;

public class ServiceRequestSpResult
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string Description { get; set; }
    public DateTimeOffset ScheduledDateTime { get; set; }
    public int Status { get; set; }
    public string Country { get; set; }
    public string City { get; set; }
    public string Area { get; set; }
    public string Street { get; set; }
    public string BuildingNumber { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    //public double DistanceKm { get; set; }
}
