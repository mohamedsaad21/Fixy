namespace Fixy.Application.Features.Admin.Queries.GetDashboard.Responses;

public class DailyBookingsResponse
{
    public DateTimeOffset Date { get; set; }
    public int Count { get; set; }
}
