using MailKit.Security;
using MimeKit;
using MailKit.Net.Smtp;
using Fixy.Infrastructure.Configurations;
using Fixy.Application.Contracts.Services;

namespace Fixy.Infrastructure.Services;

public class EmailService(EmailSettings emailSettings) : IEmailService
{
    public async Task<string> SendEmailAsync(string Email, string Message, string? reason)
    {
        try
        {
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(emailSettings.Host, emailSettings.Port, SecureSocketOptions.StartTls);
                client.Authenticate(emailSettings.FromEmail, emailSettings.Password);
                var bodybuilder = new BodyBuilder
                {
                    HtmlBody = $"{Message}",
                    TextBody = "wellcome",
                };
                var message = new MimeMessage
                {
                    Body = bodybuilder.ToMessageBody()
                };
                message.From.Add(new MailboxAddress("Fixy", emailSettings.FromEmail));
                message.To.Add(new MailboxAddress("testing", Email));
                message.Subject = reason == null ? "No Submitted" : reason;
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
            //end of sending email
            return "Success";
        }
        catch (Exception ex)
        {
            return "Failed";
        }
    }
}