using Fixy.Domain.Enums;

namespace Fixy.Application.Common.DTOs;

public record GetServiceRequestListDto
    (
        Guid Id,
        string CustomerUserName,
        string Description, 
        DateTime ScheduledDateTime,
        //List<string> ServiceCategories,
        //AddressDto Address,
        ServiceRequestStatus Status
    );
