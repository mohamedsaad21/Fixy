using AutoMapper;
using Fixy.Application.Bases;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fixy.Application.Features.ServiceCategories.Queries.GetCategoryById;

public sealed class GetCategoryByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetCategoryByIdQueryHandler> logger) : IRequestHandler<GetCategoryByIdQuery, Result<GetCategoryByIdResponse>>
{
    public async Task<Result<GetCategoryByIdResponse>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await unitOfWork.ServiceCategories.GetByIdAsync(request.Id);
        if (category == null)
        {
            logger.LogWarning("Service category fetch failed — category not found. CategoryId: {CategoryId}", request.Id);
            return Errors.UserNotFound;
        }
        logger.LogInformation("Service category fetched successfully. CategoryId: {CategoryId}, NameAr: {NameAr}, NameEn: {NameEn}", category.Id, category.NameAr, category.NameEn);
        var categoryMapper = mapper.Map<GetCategoryByIdResponse>(category);
        return categoryMapper;
    }
}
