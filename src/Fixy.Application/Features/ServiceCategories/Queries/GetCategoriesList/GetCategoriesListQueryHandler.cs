using AutoMapper;
using Fixy.Application.Bases;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fixy.Application.Features.ServiceCategories.Queries.GetCategoriesList;

public sealed class GetCategoriesListQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetCategoriesListQueryHandler> logger) : IRequestHandler<GetCategoriesListQuery, Result<List<GetCategoriesListResponse>>>
{
    public async Task<Result<List<GetCategoriesListResponse>>> Handle(GetCategoriesListQuery request, CancellationToken cancellationToken)
    {
        var categories = await unitOfWork.ServiceCategories.GetTableNoTracking().ToListAsync();
        logger.LogInformation("Service categories list fetched. Count: {Count}", categories.Count);
        var response = mapper.Map<List<GetCategoriesListResponse>>(categories);
        return response;
    }
}
