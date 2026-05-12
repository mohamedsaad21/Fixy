using Fixy.Application.Common.DTOs.ServiceRequest;
using Fixy.Application.Features.ServiceRequests.Commands.CreateServiceRequest;
using Fixy.Domain.Entities;

namespace Fixy.Application.Mapping.ServiceRequests;

public partial class ServiceRequestProfile
{
    public void AddRequestCommandToRequestDomainMapping()
    {
        CreateMap<CreateServiceRequestCommand, ServiceRequest>();
        CreateMap<AddressDto, Address>().ReverseMap();
    }
}
