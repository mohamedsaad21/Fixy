using AutoMapper;
using Fixy.Application.Bases;
using Fixy.Application.Features.ServiceCategories.Commands.Models;
using Fixy.Application.Resources;
using Fixy.Domain.Entities;
using Fixy.Infrastructure.Persistence.Abstracts;
using MediatR;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Features.ServiceCategories.Commands.Handlers;

public class CategoryCommandsHandler : IRequestHandler<AddCategoryCommand, Result<Guid>>,
                                                                           IRequestHandler<EditCategoryCommand, Result>,
                                                                           IRequestHandler<DeleteCategoryCommand, Result>
{

    private readonly IStringLocalizer<SharedResources> _stringLocalizer;
    private readonly IServiceCategoryRepository _serviceCategoryRepository;
    private readonly IMapper _mapper;

    public CategoryCommandsHandler(IStringLocalizer<SharedResources> stringLocalizer, IMapper mapper,
        IServiceCategoryRepository serviceCategoryRepository)
    {
        _stringLocalizer = stringLocalizer;
        _serviceCategoryRepository = serviceCategoryRepository;
        _mapper = mapper;
    }

    public async Task<Result<Guid>> Handle(AddCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = _mapper.Map<ServiceCategory>(request);
        await _serviceCategoryRepository.AddAsync(category);

        return category.Id;
    }

    public async Task<Result> Handle(EditCategoryCommand request, CancellationToken cancellationToken)
    {
        var oldCategory = await _serviceCategoryRepository.GetByIdAsync(request.Id);
        if (oldCategory == null) return Errors.UserNotFound;

        var newCategory = _mapper.Map(request, oldCategory);
        await _serviceCategoryRepository.SaveChangesAsync();

        return Result.Success();
    }
    // not found
    public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _serviceCategoryRepository.GetByIdAsync(request.Id);

        if (category == null) return Errors.UserNotFound;

        await _serviceCategoryRepository.DeleteAsync(category);

        return Result.Success();
    }
}