using AutoMapper;
using Fixy.Application.Bases;
using Fixy.Domain.Entities;
using Fixy.Domain.Entities.Identity;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Users.Queries.GetUserById;

public sealed class GetUserByIdQueryHandler(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IMapper mapper) : IRequestHandler<GetUserByIdQuery, Result<GetUserByIdResponse>>
{
    public async Task<Result<GetUserByIdResponse>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Id.ToString());
        if (user == null)
            return Errors.UserNotFound;

        var response = mapper.Map<GetUserByIdResponse>(user);

        if(user is Technician technician)
        {
            var serviceCategory = await unitOfWork.ServiceCategories.GetTableNoTracking()
                .FirstOrDefaultAsync(x => x.Id == technician.ServiceCategoryId);

            if (serviceCategory == null)
                return Errors.ServiceCategoryNotFound;

            response.ServiceCategory = serviceCategory.Localize(serviceCategory.NameAr, serviceCategory.NameEn);
        }

        return response;
    }
}
