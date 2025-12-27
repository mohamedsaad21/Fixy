using Fixy.Application.Bases;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Fixy.Application.Features.Authentication.Commands.Models;

public record RegisterTechnicianCommand(
    string FullName, 
    string Email, 
    string NationalId,
    int YearsOfExperience,
    IFormFile ProfilePicture,
    IFormFile NationalIdCardImage, string Password, string ConfirmPassword) : IRequest<Result<Guid>>;