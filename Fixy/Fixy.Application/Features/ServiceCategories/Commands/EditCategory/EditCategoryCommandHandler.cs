using Fixy.Application.Bases;
using Fixy.Application.Mapping.ServiceCategories;
using Fixy.Domain.Interfaces;
using MediatR;

namespace Fixy.Application.Features.ServiceCategories.Commands.EditCategory;

public sealed class EditCategoryCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<EditCategoryCommand, Result>
{
    public async Task<Result> Handle(EditCategoryCommand request, CancellationToken cancellationToken)
    {
        var oldCategory = await unitOfWork.ServiceCategories.GetByIdAsync(request.Id);
        if (oldCategory == null) return Errors.UserNotFound;

        var newCategory = request.ToServiceCategory(oldCategory);
        await unitOfWork.ServiceCategories.UpdateAsync(oldCategory);
        await unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}
