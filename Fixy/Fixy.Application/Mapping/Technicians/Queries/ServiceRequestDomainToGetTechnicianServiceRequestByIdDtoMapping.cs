using Fixy.Application.Common.DTOs;
using Fixy.Application.Features.Technicians.Queries.GetServiceRequestById;
using Fixy.Domain.Entities;

namespace Fixy.Application.Mapping.Technicians.Queries;

public static class ServiceRequestDomainToGetTechnicianServiceRequestByIdDtoMapping
{
    public static GetTechnicianServiceRequestByIdResponse ToTechnicianServiceRequestByIdResponse(this ServiceRequest serviceRequest)
    {
        return new GetTechnicianServiceRequestByIdResponse
        {
            Id = serviceRequest.Id,
            CustomerUserName = serviceRequest.Customer.UserName,
            Description = serviceRequest.Description,
            ScheduledDateTime = serviceRequest.ScheduledDateTime,
            ServiceCategories = serviceRequest.ServiceCategories.Select(x => x.Localize(x.NameAr, x.NameEn)).ToList(),
            Address = new AddressDto(serviceRequest.Address.Country, serviceRequest.Address.City, serviceRequest.Address.Area, serviceRequest.Address.Street, serviceRequest.Address.BuildingNumber, serviceRequest.Address.Latitude, serviceRequest.Address.Longitude),
            Status = serviceRequest.Status
        };
    }
}
