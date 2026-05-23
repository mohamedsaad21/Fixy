namespace Fixy.Infrastructure.Configurations;

public class FlaskApiSettings
{
    public string RatingModelUrl { get; set; }
    public string AnomalyModelUrl { get; set; }
    public string ChatbotUrl { get; set; }
    public int Timeout { get; set; }
}
