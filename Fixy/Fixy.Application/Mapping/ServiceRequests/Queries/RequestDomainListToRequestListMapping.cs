using Fixy.Application.Features.ServiceRequests.Queries.Results;
using Fixy.Domain.Entities;

namespace Fixy.Application.Mapping.ServiceRequests;

public partial class ServiceRequestProfile
{
    public void RequestDomainListToRequestListMapping()
    {
        CreateMap<ServiceRequest, GetRequestsListResponse>()
            .ForMember(t => t.CustomerUserName, opt => opt.MapFrom(src => src.Customer.UserName));
    }
}
