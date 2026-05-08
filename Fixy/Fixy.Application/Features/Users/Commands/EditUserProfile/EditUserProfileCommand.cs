using Fixy.Application.Bases;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Fixy.Application.Features.Users.Commands.EditUserProfile;

public sealed record EditUserProfileCommand
    (
        Guid UserId,
        string FirstName,
        string LastName,
        string Bio,
        IFormFile? ProfilePicture,
        string PreferredLanguage
    ) : IRequest<Result>;
