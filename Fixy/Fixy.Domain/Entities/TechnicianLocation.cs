namespace Fixy.Domain.Entities;

public class TechnicianLocation : BaseEntity
{
    public Guid TechnicianId { get; set; }
    public Technician Technician { get; set; }
    public string ServiceArea { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime UpdatedAt { get; set; }

    public TechnicianLocation(Guid technicianId, double latitude, double longitude, string serviceArea)
    {
        TechnicianId = technicianId;
        Latitude = latitude;
        Longitude = longitude;
        ServiceArea = serviceArea;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(double latitude, double longitude, string serviceArea)
    {
        Latitude = latitude;
        Longitude = longitude;
        ServiceArea = serviceArea;
        UpdatedAt = DateTime.UtcNow;
    }
}
