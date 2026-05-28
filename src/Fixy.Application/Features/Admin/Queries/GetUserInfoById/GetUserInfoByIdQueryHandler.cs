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
using Serilog;

namespace Fixy.Application.Features.Admin.Queries.GetUserInfoById;

public sealed class GetUserInfoByIdQueryHandler(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork,
    IMapper mapper, IStringLocalizer<SharedResources> localizer) : IRequestHandler<GetUserInfoByIdQuery, Result<GetUserInfoByIdResponse>>
{
    public async Task<Result<GetUserInfoByIdResponse>> Handle(GetUserInfoByIdQuery request, CancellationToken cancellationToken)
    {
        Log.Information("Admin fetching user info. UserId: {UserId}", request.UserId);

        var user = await userManager.FindByIdAsync(request.UserId.ToString());

        if (user == null)
        {
            Log.Warning("User info fetch failed — user not found. UserId: {UserId}", request.UserId);
            return Errors.UserNotFound;
        }

        var result = mapper.Map<GetUserInfoByIdResponse>(user);
        var roles = await userManager.GetRolesAsync(user);
        result.Role = localizer[roles.FirstOrDefault()!];
        if(user is Technician technician)
        {
            Log.Information("Resolving technician-specific info. UserId: {UserId}, ServiceCategoryId: {ServiceCategoryId}", request.UserId, technician.ServiceCategoryId);
            result.NationalId = technician.NationalId;
            result.NationalIdCardImageUrl = technician.NationalIdCardImageUrl;
            result.AverageRating = technician.AverageRating;
            result.Status = EnumLocalizer.Localize(technician.Status, localizer);
            // Get Service Category Of Technician
            var serviceCategory = await unitOfWork.ServiceCategories.GetTableNoTracking()
                .FirstOrDefaultAsync(x => x.Id == technician.ServiceCategoryId);

            if (serviceCategory == null)
            {
                Log.Warning("User info fetch failed — service category not found for technician. UserId: {UserId}, ServiceCategoryId: {ServiceCategoryId}", request.UserId, technician.ServiceCategoryId);
                return Errors.ServiceCategoryNotFound;
            }

            result.ServiceCategory = serviceCategory.Localize(serviceCategory.NameAr, serviceCategory.NameEn);

        } else if(user is Customer customer)
        {
            Log.Information("Resolving customer-specific info. UserId: {UserId}", request.UserId);
            result.NationalId = customer.NationalId;
            result.Status = EnumLocalizer.Localize(customer.Status, localizer);
        }
        Log.Information("User info fetched successfully. UserId: {UserId}, Role: {Role}, UserType: {UserType}", request.UserId, roles.FirstOrDefault(), user.GetType().Name);
        return result;
    }
}
