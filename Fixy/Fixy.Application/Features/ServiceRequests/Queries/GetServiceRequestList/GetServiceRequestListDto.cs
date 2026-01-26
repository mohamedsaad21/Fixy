using Fixy.Application.Common.DTOs;
using Fixy.Domain.Enums;

namespace Fixy.Application.Features.ServiceRequests.Queries.GetServiceRequestList;

public record GetServiceRequestListDto
    (
        Guid Id,
        string CustomerUserName,
        string Description, 
        DateTime ScheduledDateTime,
        List<string> ServiceCategories,
        AddressDto Address,
        ServiceRequestStatus Status
    );
