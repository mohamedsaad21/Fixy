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
    private const double GlobalMean = 3.0;
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
            .Include(x => x.ServiceRequest)
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
            RequestUri = new Uri(_flaskApiSettings.RatingModelUrl),
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

        // is anomaly code if yes put 3 in predicted rating else put rating from predict model
        var isAnomaly = await IsAnomaly(bookingId, booking.ServiceRequest.CustomerId, booking.TechnicianId, result.PredictedRating);
        booking.PredictedTechnicianRating = isAnomaly ? 3 : result.PredictedRating;
        booking.IsEvaluated = true;

        var query = _unitOfWork.Bookings.GetTableNoTracking().Where(x => x.TechnicianId == booking.TechnicianId && x.IsEvaluated);
        var totalSum = await query.SumAsync(x => x.PredictedTechnicianRating) + booking.PredictedTechnicianRating;
        var totalCount = await query.CountAsync() + 1;
        booking.Technician.AverageRating = totalSum / totalCount;
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task<bool> IsAnomaly(Guid bookingId, Guid customerId, Guid technicianId, double predictedRating)
    {
        // Pull all ratings this customer has given (excluding current booking)
        var customerRatings = await _unitOfWork.Bookings
            .GetTableNoTracking()
            .Where(x => x.ServiceRequest.CustomerId == customerId
                     && x.IsEvaluated
                     && x.PredictedTechnicianRating != null
                     && x.PredictedTechnicianRating != 0
                     && x.Id != bookingId)
            .Select(x => (double)x.PredictedTechnicianRating!.Value)
            .ToListAsync();

        var workerRatings = await _unitOfWork.Bookings
            .GetTableNoTracking()
            .Where(x => x.TechnicianId == technicianId
                     && x.IsEvaluated
                     && x.PredictedTechnicianRating != null
                     && x.PredictedTechnicianRating != 0
                     && x.Id != bookingId)
            .Select(x => (double)x.PredictedTechnicianRating!.Value)
            .ToListAsync();

        var userWorkerRatings = await _unitOfWork.Bookings
            .GetTableNoTracking()
            .Where(x => x.ServiceRequest.CustomerId == customerId
                     && x.TechnicianId == technicianId
                     && x.IsEvaluated
                     && x.PredictedTechnicianRating != null
                     && x.PredictedTechnicianRating != 0
                     && x.Id != bookingId)
            .Select(x => (double)x.PredictedTechnicianRating!.Value)
            .ToListAsync();

        double userAvgRating = 
            customerRatings.Count > 0 && customerRatings.Average() != 0? customerRatings.Average() : GlobalMean;
        double workerAvgRating = 
            workerRatings.Count > 0 && workerRatings.Average() != 0? workerRatings.Average() : GlobalMean;
        double userRatingStd = customerRatings.Count > 1 ? StdDev(customerRatings) : 0;
        double userWorkerAvg = 
            userWorkerRatings.Count > 0 && userWorkerRatings.Average() != 0? userWorkerRatings.Average() : GlobalMean;
        int feedbackCount = customerRatings.Count == 0 ? 1 : customerRatings.Count;
        double workerRatingVar = workerRatings.Count > 1 ? Variance(workerRatings) : 0;
        double ratingDeviation = predictedRating - GlobalMean;


        var requestBody = new AnomalyDetectionRequest
        {
            UserAvgRating = userAvgRating,
            WorkerAvgRating = workerAvgRating,
            UserRatingStd = userRatingStd,
            UserWorkerAvg = userWorkerAvg,
            FeedbackCount = feedbackCount,
            WorkerRatingVar = workerRatingVar,
            RatingDeviation = ratingDeviation,
            predictedRating = predictedRating
        };

        using var client = _httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(_flaskApiSettings.Timeout);

        var request = new HttpRequestMessage
        {
            RequestUri = new Uri(_flaskApiSettings.AnomalyModelUrl),
            Method = HttpMethod.Post,
            Content = new StringContent(
            JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json")
        };

        var response = await client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<AnomalyDetectionResponse>(json);

        if (result == null)
            throw new HttpRequestException("No response from Flask API.");

        if (!string.IsNullOrEmpty(result.Error))
            throw new HttpRequestException(result.Error);

        return result.PredictedAnomaly == 1;
    }

    // Helper methods — computed in memory after fetching the list
    private static double StdDev(List<double> values)
    {
        double avg = values.Average();
        double sumSquares = values.Sum(v => Math.Pow(v - avg, 2));
        return Math.Sqrt(sumSquares / (values.Count - 1)); // sample std dev, matches SQL STDEV
    }

    private static double Variance(List<double> values)
    {
        double avg = values.Average();
        return values.Sum(v => Math.Pow(v - avg, 2)) / (values.Count - 1); // matches SQL VAR
    }
}
