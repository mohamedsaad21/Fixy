using Fixy.Application.Bases;
using Fixy.Domain.Interfaces;
using MediatR;

namespace Fixy.Application.Features.ServiceCategories.Commands.DeleteCategory;

public sealed class DeleteCategoryCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteCategoryCommand, Result>
{
    public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await unitOfWork.ServiceCategories.GetByIdAsync(request.Id);

        if (category == null) return Errors.UserNotFound;

        await unitOfWork.ServiceCategories.DeleteAsync(category);
        await unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}
