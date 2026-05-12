using AutoMapper;
using Fixy.Application.Bases;
using Fixy.Application.Common.Helpers;
using Fixy.Application.Resources;
using Fixy.Domain.Entities;
using Fixy.Domain.Entities.Identity;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Features.Users.Queries.GetUserProfileById;

public sealed class GetUserProfileByIdQueryHandler : IRequestHandler<GetUserProfileByIdQuery, Result<GetUserProfileByIdResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<SharedResources> _localizer;

    public GetUserProfileByIdQueryHandler(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager,
        IMapper mapper, IStringLocalizer<SharedResources> localizer)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _mapper = mapper;
        _localizer = localizer;
    }

    public async Task<Result<GetUserProfileByIdResponse>> Handle(GetUserProfileByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Id.ToString());
        if (user == null)
            return Errors.UserNotFound;

        var response = _mapper.Map<GetUserProfileByIdResponse>(user);

        if (user is Technician technician)
        {
            response.NationalId = technician.NationalId;
            response.Status = EnumLocalizer.Localize(technician.Status, _localizer);
            // Get Service Category Of Technician
            var serviceCategory = await _unitOfWork.ServiceCategories.GetTableNoTracking()
                .FirstOrDefaultAsync(x => x.Id == technician.ServiceCategoryId);

            if (serviceCategory == null)
                return Errors.ServiceCategoryNotFound;

            response.ServiceCategory = serviceCategory.Localize(serviceCategory.NameAr, serviceCategory.NameEn);

        }
        else if (user is Customer customer)
        {
            response.NationalId = customer.NationalId;
            response.Status = EnumLocalizer.Localize(customer.Status, _localizer);
        }
        var roles = await _userManager.GetRolesAsync(user);
        response.Role = _localizer[roles.FirstOrDefault()!];
        return response;
    }
}
