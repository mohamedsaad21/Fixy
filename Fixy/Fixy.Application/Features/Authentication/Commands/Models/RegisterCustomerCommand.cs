using Fixy.Application.Bases;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Fixy.Application.Features.Authentication.Commands.Models;

public record RegisterCustomerCommand(
    string FullName,
    string Email, 
    string NationalId,
    IFormFile? ProfilePicture,
    string Password, 
    string ConfirmPassword
    ) : IRequest<Result<Guid>>;