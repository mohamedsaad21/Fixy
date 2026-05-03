namespace Fixy.Application.Common.DTOs;

public class UpdateRatingRequest
{
    public string ServiceType { get; set; }
    public string DayOfWeek { get; set; }
    public string TimeOfDay { get; set; }
    public string WeatherCondition { get; set; }
    public int JobDurationMinutes { get; set; }
    public int ResponseTimeMinutes { get; set; }
    public int PunctualityRating { get; set; }
    public int ProfessionalismRating { get; set; }
    public int CommunicationRating { get; set; }
    public int HandlingQuality { get; set; }
    public int CostSatisfaction { get; set; }
    public int CustomerBehavior { get; set; }
    public int ClarityOfIssue { get; set; }
    public int SafetyAndEnvironment { get; set; }
    public int CustomerPunctuality { get; set; }
}
