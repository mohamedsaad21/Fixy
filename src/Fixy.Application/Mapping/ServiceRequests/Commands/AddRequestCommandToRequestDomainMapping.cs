using Fixy.Application.Common.DTOs.ServiceRequest;
using Fixy.Application.Features.ServiceRequests.Commands.CreateServiceRequest;
using Fixy.Domain.Entities;
using System.Globalization;

namespace Fixy.Application.Mapping.ServiceRequests;

public partial class ServiceRequestProfile
{
    public void AddRequestCommandToRequestDomainMapping()
    {
        CreateMap<CreateServiceRequestCommand, ServiceRequest>();
        CreateMap<AddressDto, Address>()
            .ForMember(x => x.Latitude, opt => opt.MapFrom(src => double.Parse(src.Latitude, CultureInfo.InvariantCulture)))
            .ForMember(x => x.Longitude, opt => opt.MapFrom(src => double.Parse(src.Longitude, CultureInfo.InvariantCulture)));
        CreateMap<Address, AddressDto>();
    }
}
