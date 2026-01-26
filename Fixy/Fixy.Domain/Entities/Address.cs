namespace Fixy.Domain.Entities;

public class Address
{
    public string Country { get; set; }
    public string City { get; set; }
    public string Area { get; set; }
    public string Street { get; set; }
    public string BuildingNumber { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    private Address() { }

    public Address(string country, string city, string area, string street, string buildingNumber, double? latitude, double? longitude)
    {
        Country = country;
        City = city;
        Area = area;
        Street = street;
        BuildingNumber = buildingNumber;
        Latitude = latitude;
        Longitude = longitude;
    }
}
