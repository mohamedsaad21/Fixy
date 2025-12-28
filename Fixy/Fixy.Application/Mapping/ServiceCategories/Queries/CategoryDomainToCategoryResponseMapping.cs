using Fixy.Application.Features.ServiceCategory.Queries.Results;
using Fixy.Domain.Entities;

namespace Fixy.Application.Mapping.ServiceCategories;

public partial class ServiceCategoriesProfile
{
    public void CategoryDomainToCategoryResponseMapping()
    {
        CreateMap<ServiceCategory, GetCategoryByIdResponse>();
    }
}
