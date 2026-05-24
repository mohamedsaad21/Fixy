namespace Fixy.Application.Features.Users.Queries.GetCurrentUser;

public class GetCurrentUserResponse
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
}
