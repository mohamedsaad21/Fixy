using System.Text.Json.Serialization;

namespace Fixy.Application.Common.DTOs.RatingPrediction;

public class AnomalyDetectionResponse
{
    [JsonPropertyName("predicted_anomaly")]
    public int PredictedAnomaly { get; set; }
    [JsonPropertyName("error")]
    public string? Error { get; set; }
}
