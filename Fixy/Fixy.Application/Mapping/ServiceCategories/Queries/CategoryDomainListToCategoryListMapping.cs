using Fixy.Application.Features.ServiceCategories.Queries.Results;
using Fixy.Domain.Entities;

namespace Fixy.Application.Mapping.ServiceCategories;

public partial class ServiceCategoriesProfile
{
    public void CategoryDomainListToCategoryListResponseMapping()
    {
        CreateMap<ServiceCategory, GetCategoriesListResponse>();
    }
}
