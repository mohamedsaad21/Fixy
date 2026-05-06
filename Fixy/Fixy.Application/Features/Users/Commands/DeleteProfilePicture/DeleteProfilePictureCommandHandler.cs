using Fixy.Application.Bases;
using Fixy.Application.Contracts.ExternalServices;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Fixy.Application.Features.Users.Commands.DeleteProfilePicture;

public sealed class DeleteProfilePictureCommandHandler(ICurrentUserService currentUserService, IStorageService fileService, UserManager<ApplicationUser> userManager) : IRequestHandler<DeleteProfilePictureCommand, Result>
{
    public async Task<Result> Handle(DeleteProfilePictureCommand request, CancellationToken cancellationToken)
    {
        var user = await currentUserService.GetCurrentUserAsync();

        if(user.ProfilePictureUrl == null)
            return Errors.AlreadyNoProfilePictureExists;

        await fileService.DeleteAsync(user.ProfilePictureUrl);
        user.ProfilePictureUrl = null;
        await userManager.UpdateAsync(user);
        return Result.Success();
    }
}
