namespace Fixy.Application.Features.ServiceCategories.Queries.Results;

public record GetCategoriesListResponse
(
    Guid Id,
    string Name,
    string Description
);