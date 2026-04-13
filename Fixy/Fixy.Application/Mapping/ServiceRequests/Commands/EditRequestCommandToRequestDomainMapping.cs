using Fixy.Application.Common.DTOs;
using Fixy.Application.Features.ServiceRequests.Commands.EditServiceRequest;
using Fixy.Application.Features.ServiceRequests.Queries.GetServiceRequestById;
using Fixy.Application.Mapping.PriceOffers.Queries;
using Fixy.Domain.Entities;
using Fixy.Domain.Enums;

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
