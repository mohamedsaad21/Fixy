using Fixy.Application.Features.ServiceCategories.Commands.AddCategory;
using Fixy.Domain.Entities;

namespace Fixy.Application.Mapping.ServiceCategories;

public partial class ServiceCategoriesProfile
{
    public void AddCategoryCommandToCommandDomainMapping()
    {
        CreateMap<AddCategoryCommand, ServiceCategory>();
    }
}
