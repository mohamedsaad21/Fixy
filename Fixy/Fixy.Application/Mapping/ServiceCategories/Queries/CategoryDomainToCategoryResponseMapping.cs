using Fixy.Application.Features.ServiceCategories.Queries.GetCategoryById;
using Fixy.Domain.Entities;

namespace Fixy.Application.Mapping.ServiceCategories;

public partial class ServiceCategoriesProfile
{
    public void CategoryDomainToCategoryResponseMapping()
    {
        CreateMap<ServiceCategory, GetCategoryByIdResponse>();
    }
}
