namespace Fixy.Application.Features.Users.Queries.GetUserProfileById;

public class GetUserProfileByIdResponse
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string NationalId { get; set; }
    public string ProfilePictureUrl { get; set; }
    public string Role { get; set; }
    public string Bio { get; set; }
    public string PreferredLanguage { get; set; }
    public string Status { get; set; }
    public bool IsTwoFactorEmailEnabled { get; set; }
    public string ServiceCategory { get; set; }
}
