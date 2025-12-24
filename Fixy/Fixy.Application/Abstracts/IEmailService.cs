namespace Fixy.Application.Abstracts;

public interface IEmailService
{
    Task<string> SendEmailAsync(string Email, string Message, string? reason);
}
