using Fixy.Application.Features.ServiceCategories.Queries.GetCategoriesList;
using Fixy.Domain.Entities;

namespace Fixy.Application.Mapping.ServiceCategories;

public partial class ServiceCategoriesProfile
{
    public void CategoryDomainListToCategoryListResponseMapping()
    {
        CreateMap<ServiceCategory, GetCategoriesListResponse>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Localize(src.NameAr, src.NameEn)));
    }
}
