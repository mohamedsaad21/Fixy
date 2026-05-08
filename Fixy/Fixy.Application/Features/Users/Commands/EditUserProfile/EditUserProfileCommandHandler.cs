using AutoMapper;
using Fixy.Application.Bases;
using Fixy.Application.Contracts.ExternalServices;
using Fixy.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Fixy.Application.Features.Users.Commands.EditUserProfile;

public sealed class EditUserProfileCommandHandler(UserManager<ApplicationUser> userManager, IMapper mapper, IStorageService storageService) : IRequestHandler<EditUserProfileCommand, Result>
{
    public async Task<Result> Handle(EditUserProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString());

        if (user == null)
            return Errors.UserNotFound;

        user = mapper.Map(request, user);

        if (request.ProfilePicture != null)
        {
            // Delete old profile picture
            await storageService.DeleteAsync(user.ProfilePictureUrl!);
            // Set new profile picture
            user.ProfilePictureUrl = await storageService.UploadAsync(request.ProfilePicture);
        }
        await userManager.UpdateAsync(user);
        return Result.Success();
    }
}
