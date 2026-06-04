namespace Fixy.Application.Common.DTOs.RatingPrediction;

public class AnomalyDetectionRequest
{
    public double UserAvgRating { get; set; }
    public double WorkerAvgRating { get; set; }
    public double UserRatingStd { get; set; }
    public double UserWorkerAvg { get; set; }
    public int FeedbackCount { get; set; }
    public double WorkerRatingVar { get; set; }
    public double RatingDeviation { get; set; }
    public double PredictedRating { get; set; }
}
