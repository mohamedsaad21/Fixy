using AutoMapper;

namespace Fixy.Application.Mapping.ServiceCategories;

public partial class ServiceCategoriesProfile : Profile
{
    public ServiceCategoriesProfile()
    {
        AddCategoryCommandToCommandDomainMapping();
        CategoryDomainListToCategoryListResponseMapping();
        CategoryDomainToCategoryResponseMapping();
    }
}
