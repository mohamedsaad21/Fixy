namespace Fixy.Application.Features.Users.Queries.GetUserById;

public class GetUserByIdResponse
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string ProfilePictureUrl { get; set; }
    public bool IsTwoFactorEmailEnabled { get; set; }
}
