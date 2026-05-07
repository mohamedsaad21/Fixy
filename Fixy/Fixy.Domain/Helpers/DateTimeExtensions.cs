namespace Fixy.Domain.Helpers;

public static class DateTimeExtensions
{
    private static readonly TimeZoneInfo EgyptTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Africa/Cairo");

    public static DateTimeOffset ToEgyptTime(this DateTimeOffset utcDate)
    {
        return TimeZoneInfo.ConvertTime(utcDate, EgyptTimeZone);
    }
}