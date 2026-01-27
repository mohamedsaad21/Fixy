using Fixy.Domain.Enums;

namespace Fixy.Application.Features.ServiceRequests.Queries.GetServiceRequestById;

public record PriceOfferDto
    (
        Guid Id,
        string TechnicianUserName,
        decimal MinPrice,
        decimal MaxPrice,
        DateTime CreatedAt
    );
