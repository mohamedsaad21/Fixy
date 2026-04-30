using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Fixy.Application.Features.Users.Commands.EditLanguage;

public sealed class EditLanguageCommandHandler(ICurrentUserService currentUserService, UserManager<ApplicationUser> userManager) : IRequestHandler<EditLanguageCommand, Result>
{
    public async Task<Result> Handle(EditLanguageCommand request, CancellationToken cancellationToken)
    {
        var currentUser = await currentUserService.GetCurrentUserAsync();

        if (currentUser == null)
            return Errors.Unauthorized;

        currentUser.PreferredLanguage = request.Language;
        await userManager.UpdateAsync(currentUser);
        return Result.Success();
    }
}
