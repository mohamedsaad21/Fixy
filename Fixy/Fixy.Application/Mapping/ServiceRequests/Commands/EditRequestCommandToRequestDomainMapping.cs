using Fixy.Application.Features.ServiceRequests.Commands.EditServiceRequest;
using Fixy.Domain.Entities;

namespace Fixy.Application.Mapping.ServiceRequests.Commands;

public static class EditRequestCommandToRequestDomainMapping
{
    public static ServiceRequest ToServiceRequestDomain(this EditServiceRequestCommand command, ServiceRequest serviceRequest)
    {
        serviceRequest.Description = command.Description;
        serviceRequest.ScheduledDateTime = command.ScheduledDateTime;
        serviceRequest.Address.Country = command.Address.Country;
        serviceRequest.Address.City = command.Address.City;
        serviceRequest.Address.Area = command.Address.Area;
        serviceRequest.Address.Street = command.Address.Street;
        serviceRequest.Address.BuildingNumber = command.Address.BuildingNumber;
        serviceRequest.Address.Latitude = command.Address.Latitude;
        serviceRequest.Address.Longitude = command.Address.Longitude;
        return serviceRequest;
    }
}
