using Fixy.Application.Features.ServiceCategories.Commands.EditCategory;
using Fixy.Domain.Entities;

namespace Fixy.Application.Mapping.ServiceCategories;

public static class EditCategoryCommandToCommandDomainMapping
{
    public static ServiceCategory ToServiceCategory(this EditCategoryCommand command, ServiceCategory serviceCategory)
    {
        serviceCategory.Name = command.Name;
        serviceCategory.Description = command.Description;
        serviceCategory.UpdatedAt = DateTime.UtcNow;
        return serviceCategory;
    }
}
