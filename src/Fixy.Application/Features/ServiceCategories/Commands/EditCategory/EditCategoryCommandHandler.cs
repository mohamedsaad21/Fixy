using Fixy.Application.Bases;
using Fixy.Application.Mapping.ServiceCategories;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fixy.Application.Features.ServiceCategories.Commands.EditCategory;

public sealed class EditCategoryCommandHandler(IUnitOfWork unitOfWork, ILogger<EditCategoryCommandHandler> logger) : IRequestHandler<EditCategoryCommand, Result>
{
    public async Task<Result> Handle(EditCategoryCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Admin attempting to edit service category. CategoryId: {CategoryId}", request.Id);
        var oldCategory = await unitOfWork.ServiceCategories.GetByIdAsync(request.Id);
        
        if (oldCategory == null)
        {
            logger.LogWarning("Service category edit failed — category not found. CategoryId: {CategoryId}", request.Id);
            return Errors.UserNotFound;
        }
        var previousNameAr = oldCategory.NameAr;
        var previousNameEn = oldCategory.NameEn;
        var newCategory = request.ToServiceCategory(oldCategory);
        await unitOfWork.ServiceCategories.UpdateAsync(oldCategory);
        await unitOfWork.SaveChangesAsync();

        logger.LogInformation("Service category updated successfully. CategoryId: {CategoryId}, PreviousNameAr: {PreviousNameAr}, PreviousNameEn: {PreviousNameEn}, NewNameAr: {NewNameAr}, NewNameEn: {NewNameEn}",
            oldCategory.Id, previousNameAr, previousNameEn, oldCategory.NameAr, oldCategory.NameEn);

        return Result.Success();
    }
}
