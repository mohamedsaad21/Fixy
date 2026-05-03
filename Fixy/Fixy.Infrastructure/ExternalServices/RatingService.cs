using Fixy.Application.Common.DTOs.RatingPrediction;
using Fixy.Application.Contracts.ExternalServices;
using Fixy.Domain.Interfaces;
using Fixy.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;

namespace Fixy.Infrastructure.ExternalServices;

public class RatingService : IRatingService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly FlaskApiSettings _flaskApiSettings;
    private readonly IUnitOfWork _unitOfWork;
    public RatingService(IHttpClientFactory httpClientFactory, FlaskApiSettings flaskApiSettings, IUnitOfWork unitOfWork)
    {
        _httpClientFactory = httpClientFactory;
        _flaskApiSettings = flaskApiSettings;
        _unitOfWork = unitOfWork;
    }
    public async Task PredictTechnicianRatingAsync(Guid bookingId)
    {
        var booking = await _unitOfWork.Bookings
            .GetTableAsTracking().Include(x => x.Technician).ThenInclude(x => x.ServiceCategory)
            .FirstOrDefaultAsync(x => x.Id == bookingId);

        if (booking == null)
            throw new KeyNotFoundException($"Booking {bookingId} not found.");

        var customerFeedback = await _unitOfWork.CustomerFeedbacks
            .GetTableNoTracking()
            .FirstOrDefaultAsync(x => x.ServiceBookingId == booking.Id);

        var technicianFeedback = await _unitOfWork.TechnicianFeedbacks
            .GetTableNoTracking()
            .FirstOrDefaultAsync(x => x.ServiceBookingId == booking.Id);

        if (customerFeedback == null || technicianFeedback == null)
            throw new InvalidOperationException($"Feedback not complete for booking {bookingId}.");

        var requestBody = new RatingPredictionRequest
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
        var result = JsonSerializer.Deserialize<RatingPredictionResponse>(json);

        if (result == null)
            throw new HttpRequestException("No response from Flask API.");

        if(!string.IsNullOrEmpty(result.Error))
            throw new HttpRequestException(result.Error);

        booking.PredictedTechnicianRating = result.PredictedRating;
        booking.IsEvaluated = true;

        var query = _unitOfWork.Bookings.GetTableNoTracking().Where(x => x.TechnicianId == booking.TechnicianId && x.IsEvaluated);
        var totalSum = await query.SumAsync(x => x.PredictedTechnicianRating) + booking.PredictedTechnicianRating;
        var totalCount = await query.CountAsync() + 1;
        booking.Technician.AverageRating = totalSum / totalCount;
        await _unitOfWork.SaveChangesAsync();
    }
}
