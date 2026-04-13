using AutoMapper;
using Fixy.Application.Bases;
using Fixy.Domain.Entities;
using Fixy.Domain.Interfaces;
using MediatR;

namespace Fixy.Application.Features.ServiceCategories.Commands.AddCategory;

public sealed class AddCategoryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<AddCategoryCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(AddCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = mapper.Map<ServiceCategory>(request);
        await unitOfWork.ServiceCategories.AddAsync(category);
        await unitOfWork.SaveChangesAsync();
        return category.Id;
    }
}
