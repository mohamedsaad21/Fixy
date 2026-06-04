namespace Fixy.Application.Common.DTOs.ServiceRequest;

public record AddressDto
  (
        string Country,
        string City, 
        string Area, 
        string Street, 
        string BuildingNumber, 
        string Latitude, 
        string Longitude
  );
