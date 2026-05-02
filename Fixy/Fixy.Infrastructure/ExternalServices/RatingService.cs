using Fixy.Application.Common.DTOs;
using Fixy.Application.Contracts.ExternalServices;
using Fixy.Domain.Entities;
using Fixy.Domain.Entities.Feedback;
using Fixy.Infrastructure.Configurations;
using System.Text;
using System.Text.Json;

namespace Fixy.Infrastructure.ExternalServices;

public class RatingService : IRatingService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly FlaskApiSettings _flaskApiSettings;
    public RatingService(IHttpClientFactory httpClientFactory, FlaskApiSettings flaskApiSettings)
    {
        _httpClientFactory = httpClientFactory;
        _flaskApiSettings = flaskApiSettings;
    }
    public async Task UpdateTechnicianRatingAsync(ServiceBooking booking, CustomerFeedback customerFeedback, TechnicianFeedback technicianFeedback)
    {
        var requestBody = new UpdateRatingRequest
        {
            ServiceType = booking.Technician.ServiceCategory.Localize(booking.Technician.ServiceCategory.NameAr, booking.Technician.ServiceCategory.NameEn),
            DayOfWeek = booking.ScheduledDateTime.DayOfWeek.ToString(),
            TimeOfDay = booking.ScheduledDateTime.TimeOfDay.ToString(),
            WeatherCondition = customerFeedback.WeatherCondition.ToString(),
            JobDurationMinutes = customerFeedback.JobDurationMinutes,
            ResponseTimeMinutes = customerFeedback.ResponseTimeMinutes,
            PunctualityRating = customerFeedback.PunctualityRating,
            ProfessionalismRating = customerFeedback.ProfessionalismRating,
            CommunicationRating = customerFeedback.CommunicationRating,
            HandlingQuality = customerFeedback.HandlingQuality,
            CostSatisfaction = customerFeedback.CostSatisfaction,
            CustomerBehavior = technicianFeedback.CustomerBehaviorRating,
            ClarityOfIssue = technicianFeedback.ClarityOfIssue,
            SafetyAndEnvironment = technicianFeedback.SafetyAndEnvironment,
            CustomerPunctuality = technicianFeedback.CustomerPunctuality
        };

        using var client = _httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(_flaskApiSettings.Timeout);

        var request = new HttpRequestMessage
        {
            RequestUri = new Uri(_flaskApiSettings.BaseUrl),
            Method = HttpMethod.Post,
            Content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json")
        };

        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var responseDictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

        //booking.Technician.Rating = newRating;
        //booking.Technician.TotalReviews += 1;
        //booking.IsReviewed = true;
    }
}
