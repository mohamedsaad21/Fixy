using Fixy.Application.Bases;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fixy.Application.Features.ServiceCategories.Commands.DeleteCategory;

public sealed class DeleteCategoryCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteCategoryCommandHandler> logger) : IRequestHandler<DeleteCategoryCommand, Result>
{
    public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Admin attempting to delete service category. CategoryId: {CategoryId}", request.Id);
        var category = await unitOfWork.ServiceCategories.GetByIdAsync(request.Id);

        if (category == null)
        {
            logger.LogWarning("Service category deletion failed — category not found. CategoryId: {CategoryId}", request.Id);
            return Errors.UserNotFound;
        }

        await unitOfWork.ServiceCategories.DeleteAsync(category);
        await unitOfWork.SaveChangesAsync();

        logger.LogInformation("Service category deleted successfully. CategoryId: {CategoryId}, NameAr: {NameAr}, NameEn: {NameEn}", category.Id, category.NameAr, category.NameEn);

        return Result.Success();
    }
}
