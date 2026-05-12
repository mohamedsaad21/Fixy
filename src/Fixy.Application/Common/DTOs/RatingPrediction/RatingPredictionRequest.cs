using System.Text.Json.Serialization;

namespace Fixy.Application.Common.DTOs.RatingPrediction;

public class RatingPredictionRequest
{
    [JsonPropertyName("service_type")]
    public string ServiceType { get; set; }
    [JsonPropertyName("day_of_week")]
    public string DayOfWeek { get; set; }
    [JsonPropertyName("time_of_day")]
    public string TimeOfDay { get; set; }
    [JsonPropertyName("weather_condition")]
    public string WeatherCondition { get; set; }
    [JsonPropertyName("job_duration_minutes")]
    public int JobDurationMinutes { get; set; }
    [JsonPropertyName("response_time_minutes")]
    public int ResponseTimeMinutes { get; set; }
    [JsonPropertyName("punctuality_rating")]
    public int PunctualityRating { get; set; }
    [JsonPropertyName("professionalism_rating")]
    public int ProfessionalismRating { get; set; }
    [JsonPropertyName("communication_rating")]
    public int CommunicationRating { get; set; }
    [JsonPropertyName("handling_quality")]
    public int HandlingQuality { get; set; }
    [JsonPropertyName("cost_satisfaction")]
    public int CostSatisfaction { get; set; }
    [JsonPropertyName("customer_behavior")]
    public int CustomerBehavior { get; set; }
    [JsonPropertyName("clarity_of_issue")]
    public int ClarityOfIssue { get; set; }
    [JsonPropertyName("safety_and_environment")]
    public int SafetyAndEnvironment { get; set; }
    [JsonPropertyName("customer_punctuality")]
    public int CustomerPunctuality { get; set; }
}
