using Fixy.Application.Features.ServiceRequests.Commands.EditServiceRequest;
using Fixy.Domain.Entities;
using System.Globalization;

namespace Fixy.Application.Mapping.ServiceRequests.Commands;

public static class EditRequestCommandToRequestDomainMapping
{
    public static ServiceRequest ToServiceRequestDomain(this EditServiceRequestCommand command,
        ServiceRequest serviceRequest, List<ServiceCategory> newCategories)
    {
        serviceRequest.Description = command.Description;
        serviceRequest.ScheduledDateTime = command.ScheduledDateTime;
        serviceRequest.Address.Country = command.Address.Country;
        serviceRequest.Address.City = command.Address.City;
        serviceRequest.Address.Area = command.Address.Area;
        serviceRequest.Address.Street = command.Address.Street;
        serviceRequest.Address.BuildingNumber = command.Address.BuildingNumber;
        serviceRequest.Address.Latitude = double.Parse(command.Address.Latitude, CultureInfo.InvariantCulture);
        serviceRequest.Address.Longitude = double.Parse(command.Address.Longitude, CultureInfo.InvariantCulture);
        // Remove categories that are no longer selected
        var toRemove = serviceRequest.ServiceCategories
            .Where(c => !command.ServiceCategories.Contains(c.Id))
            .ToList();
        foreach (var cat in toRemove)
            serviceRequest.ServiceCategories.Remove(cat);

        // Add newly selected categories that aren't already tracked
        var existingIds = serviceRequest.ServiceCategories.Select(c => c.Id).ToHashSet();
        foreach (var cat in newCategories.Where(c => !existingIds.Contains(c.Id)))
            serviceRequest.ServiceCategories.Add(cat);

        return serviceRequest;
    }
}
