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

namespace Fixy.Application.Features.Admin.Queries.GetUserInfoById;

public sealed class GetUserInfoByIdQueryHandler(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork,
    IMapper mapper, IStringLocalizer<SharedResources> localizer) : IRequestHandler<GetUserInfoByIdQuery, Result<GetUserInfoByIdResponse>>
{
    public async Task<Result<GetUserInfoByIdResponse>> Handle(GetUserInfoByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString());

        if (user == null)
            return Errors.UserNotFound;

        var result = mapper.Map<GetUserInfoByIdResponse>(user);
        var roles = await userManager.GetRolesAsync(user);
        result.Role = roles.FirstOrDefault()!;
        if(user is Technician technician)
        {
            result.NationalId = technician.NationalId;
            result.NationalIdCardImageUrl = technician.NationalIdCardImageUrl;
            result.AverageRating = technician.AverageRating;
            result.Status = EnumLocalizer.Localize(technician.Status, localizer);
            // Get Service Category Of Technician
            var serviceCategory = await unitOfWork.ServiceCategories.GetTableNoTracking()
                .FirstOrDefaultAsync(x => x.Id == technician.ServiceCategoryId);

            if (serviceCategory == null)
                return Errors.ServiceCategoryNotFound;

            result.ServiceCategory = serviceCategory.Localize(serviceCategory.NameAr, serviceCategory.NameEn);

        } else if(user is Customer customer)
        {
            result.NationalId = customer.NationalId;
            result.Status = EnumLocalizer.Localize(customer.Status, localizer);
        }
        return result;
    }
}
