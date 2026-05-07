namespace Fixy.Application.Features.Admin.Queries.GetUserInfoById;

public class GetUserInfoByIdResponse
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string UserName { get; set; }
    public string Role { get; set; }
    public string ProfilePictureUrl { get; set; }
    public double CancellationRate { get; set; }
    public DateTimeOffset JoinDate { get; set; }
    public string NationalId { get; set; }
    // For Tech
    public string NationalIdCardImageUrl { get; set; }
    public double AverageRating { get; set; }
    public int CompletedBookings { get; set; }

}
