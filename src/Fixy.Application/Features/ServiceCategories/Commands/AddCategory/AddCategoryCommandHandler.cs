using AutoMapper;
using Fixy.Application.Bases;
using Fixy.Domain.Entities;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fixy.Application.Features.ServiceCategories.Commands.AddCategory;

public sealed class AddCategoryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<AddCategoryCommandHandler> logger) : IRequestHandler<AddCategoryCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(AddCategoryCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Admin adding new service category. NameAr: {NameAr}, NameEn: {NameEn}", request.NameAr, request.NameEn);
        var category = mapper.Map<ServiceCategory>(request);
        await unitOfWork.ServiceCategories.AddAsync(category);
        await unitOfWork.SaveChangesAsync();
        logger.LogInformation("Service category added successfully. CategoryId: {CategoryId}, NameAr: {NameAr}, NameEn: {NameEn}", category.Id, category.NameAr, category.NameEn);
        return category.Id;
    }
}
