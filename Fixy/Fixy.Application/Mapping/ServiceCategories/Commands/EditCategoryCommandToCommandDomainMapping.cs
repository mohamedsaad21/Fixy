using Fixy.Application.Features.ServiceCategories.Commands.Models;
using Fixy.Domain.Entities;

namespace Fixy.Application.Mapping.ServiceCategories;

public partial class ServiceCategoriesProfile
{
    public void EditCategoryCommandToCommandDomainMapping()
    {
        CreateMap<EditCategoryCommand, ServiceCategory>();
    }
}
