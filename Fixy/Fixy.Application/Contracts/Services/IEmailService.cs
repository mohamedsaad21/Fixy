namespace Fixy.Application.Contracts.Services;

public interface IEmailService
{
    Task<string> SendEmailAsync(string Email, string Message, string? reason);
}
