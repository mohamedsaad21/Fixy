namespace Fixy.Application.Features.Users.Queries.GetUserById;

public class GetUserByIdResponse
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string ProfilePictureUrl { get; set; }
    public bool IsTwoFactorEmailEnabled { get; set; }
    public string ServiceCategory { get; set; }
}
