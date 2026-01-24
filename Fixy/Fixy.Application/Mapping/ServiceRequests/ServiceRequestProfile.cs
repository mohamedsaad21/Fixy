using AutoMapper;

namespace Fixy.Application.Mapping.ServiceRequests;

public partial class ServiceRequestProfile : Profile
{
    public ServiceRequestProfile()
    {
        AddRequestCommandToRequestDomainMapping();
        RequestDomainListToRequestListMapping();
    }
}
