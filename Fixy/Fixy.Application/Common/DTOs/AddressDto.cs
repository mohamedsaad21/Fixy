namespace Fixy.Application.Common.DTOs;

public record AddressDto
  (
        string Country, 
        string City, 
        string Area, 
        string Street, 
        string BuildingNumber, 
        double Latitude, 
        double Longitude
  );
