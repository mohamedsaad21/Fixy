using AutoMapper;
using Fixy.Application.Bases;
using Fixy.Application.Features.ServiceCategories.Commands.Models;
using Fixy.Application.Mapping.ServiceCategories;
using Fixy.Application.Resources;
using Fixy.Domain.Entities;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Features.ServiceCategories.Commands.Handlers;

public class CategoryCommandsHandler : IRequestHandler<AddCategoryCommand, Result<Guid>>,
                                                                           IRequestHandler<EditCategoryCommand, Result>,
                                                                           IRequestHandler<DeleteCategoryCommand, Result>
{

    private readonly IStringLocalizer<SharedResources> _stringLocalizer;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CategoryCommandsHandler(IStringLocalizer<SharedResources> stringLocalizer, IMapper mapper, IUnitOfWork unitOfWork)
    {
        _stringLocalizer = stringLocalizer;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(AddCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = _mapper.Map<ServiceCategory>(request);
        await _unitOfWork.ServiceCategories.AddAsync(category);
        await _unitOfWork.SaveChangesAsync();
        return category.Id;
    }

    public async Task<Result> Handle(EditCategoryCommand request, CancellationToken cancellationToken)
    {
        var oldCategory = await _unitOfWork.ServiceCategories.GetByIdAsync(request.Id);
        if (oldCategory == null) return Errors.UserNotFound;

        var newCategory = request.ToServiceCategory(oldCategory);
        await _unitOfWork.ServiceCategories.UpdateAsync(oldCategory);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
    // not found
    public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _unitOfWork.ServiceCategories.GetByIdAsync(request.Id);

        if (category == null) return Errors.UserNotFound;

        await _unitOfWork.ServiceCategories.DeleteAsync(category);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}