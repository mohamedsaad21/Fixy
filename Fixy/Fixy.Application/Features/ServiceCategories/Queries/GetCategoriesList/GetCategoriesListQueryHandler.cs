using AutoMapper;
using Fixy.Application.Bases;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.ServiceCategories.Queries.GetCategoriesList;

public sealed class GetCategoriesListQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetCategoriesListQuery, Result<List<GetCategoriesListResponse>>>
{
    public async Task<Result<List<GetCategoriesListResponse>>> Handle(GetCategoriesListQuery request, CancellationToken cancellationToken)
    {
        var categories = await unitOfWork.ServiceCategories.GetTableNoTracking().ToListAsync();
        var response = mapper.Map<List<GetCategoriesListResponse>>(categories);
        return response;
    }
}
