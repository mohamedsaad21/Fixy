using System.Text.Json.Serialization;

namespace Fixy.Application.Common.DTOs.RatingPrediction;

public class RatingPredictionResponse
{
    [JsonPropertyName("predicted_rating")]
    public int PredictedRating { get; set; }
    [JsonPropertyName("error")]
    public string? Error { get; set; }
}
