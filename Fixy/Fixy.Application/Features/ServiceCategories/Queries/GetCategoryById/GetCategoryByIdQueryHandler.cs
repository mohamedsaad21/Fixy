using AutoMapper;
using Fixy.Application.Bases;
using Fixy.Domain.Interfaces;
using MediatR;

namespace Fixy.Application.Features.ServiceCategories.Queries.GetCategoryById;

public sealed class GetCategoryByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetCategoryByIdQuery, Result<GetCategoryByIdResponse>>
{
    public async Task<Result<GetCategoryByIdResponse>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await unitOfWork.ServiceCategories.GetByIdAsync(request.Id);
        if (category == null) return Errors.UserNotFound;

        var categoryMapper = mapper.Map<GetCategoryByIdResponse>(category);
        return categoryMapper;
    }
}
