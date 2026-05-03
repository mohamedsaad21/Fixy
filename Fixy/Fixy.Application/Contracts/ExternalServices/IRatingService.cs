namespace Fixy.Application.Contracts.ExternalServices;

public interface IRatingService
{
    Task PredictTechnicianRatingAsync(Guid bookingId);
}
