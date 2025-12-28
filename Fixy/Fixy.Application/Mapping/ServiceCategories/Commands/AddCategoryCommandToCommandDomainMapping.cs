using Fixy.Application.Features.ServiceCategory.Commands.Models;
using Fixy.Domain.Entities;

namespace Fixy.Application.Mapping.ServiceCategories;

public partial class ServiceCategoriesProfile
{
    public void AddCategoryCommandToCommandDomainMapping()
    {
        CreateMap<AddCategoryCommand, ServiceCategory>();
    }
}
