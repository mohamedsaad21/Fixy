namespace Fixy.Application.Features.ServiceCategories.Queries.Results;

public class GetCategoryByIdResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}
