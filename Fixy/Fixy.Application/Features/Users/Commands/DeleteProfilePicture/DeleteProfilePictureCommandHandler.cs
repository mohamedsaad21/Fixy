using Fixy.Application.Bases;
using Fixy.Application.Contracts.ExternalServices;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Fixy.Application.Features.Users.Commands.DeleteProfilePicture;

public sealed class DeleteProfilePictureCommandHandler(ICurrentUserService currentUserService, IFileService fileService, UserManager<ApplicationUser> userManager) : IRequestHandler<DeleteProfilePictureCommand, Result>
{
    public async Task<Result> Handle(DeleteProfilePictureCommand request, CancellationToken cancellationToken)
    {
        var user = await currentUserService.GetCurrentUserAsync();

        if(user.ProfilePictureUrl == null)
            return Errors.AlreadyNoProfilePictureExists;

        await fileService.DeleteAsync(user.ProfilePicturePublicId);
        user.ProfilePictureUrl = null;
        user.ProfilePicturePublicId = null;
        await userManager.UpdateAsync(user);
        return Result.Success();
    }
}
