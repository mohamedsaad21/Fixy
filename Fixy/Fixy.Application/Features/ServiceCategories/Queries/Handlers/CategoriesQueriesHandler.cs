using AutoMapper;
using Fixy.Application.Bases;
using Fixy.Application.Features.ServiceCategories.Queries.Models;
using Fixy.Application.Features.ServiceCategories.Queries.Results;
using Fixy.Application.Resources;
using Fixy.Infrastructure.Persistence.Abstracts;
using MediatR;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Features.ServiceCategories.Queries.Handlers;

public class CategoriesQueriesHandler : IRequestHandler<GetCategoriesListQuery, Result<List<GetCategoriesListResponse>>>,
                                                                        IRequestHandler<GetCategoryByIdQuery, Result<GetCategoryByIdResponse>>
{

    private readonly IStringLocalizer<SharedResources> _stringLocalizer;
    private readonly IServiceCategoryReadRepository _serviceCategoryReadRepository;
    private readonly IMapper _mapper;

    public CategoriesQueriesHandler(IStringLocalizer<SharedResources> stringLocalizer, IMapper mapper,
        IServiceCategoryReadRepository serviceCategoryReadRepository)
    {
        _stringLocalizer = stringLocalizer;
        _serviceCategoryReadRepository = serviceCategoryReadRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<GetCategoriesListResponse>>> Handle(GetCategoriesListQuery request, CancellationToken cancellationToken)
    {
        var categories = await _serviceCategoryReadRepository.GetServiceCategories();
        var response = _mapper.Map<List<GetCategoriesListResponse>>(categories);
        return response;
    }

    public async Task<Result<GetCategoryByIdResponse>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await _serviceCategoryReadRepository.GetServiceCategoryById(request.Id);
        if (category == null) return Errors.UserNotFound;

        var mapper = _mapper.Map<GetCategoryByIdResponse>(category);
        return mapper;
    }
}