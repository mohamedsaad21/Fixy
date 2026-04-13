using Fixy.Application.Bases;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Fixy.Application.Features.Authentication.Commands.RegisterTechnician;

public sealed record RegisterTechnicianCommand(
    string FullName, 
    string Email, 
    string NationalId,
    int YearsOfExperience,
    Guid ServiceCategoryId,
    IFormFile? ProfilePicture,
    IFormFile NationalIdCardImage, 
    string Password, 
    string ConfirmPassword
    ) : IRequest<Result<Guid>>;