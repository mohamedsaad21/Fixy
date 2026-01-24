using Fixy.Application.Common.DTOs;

namespace Fixy.Application.Features.ServiceRequests.Queries.Results;

public record GetRequestsListResponse(string CustomerUserName, string Description, DateTime ScheduledDateTime, AddressDto Adress);
