using Fixy.Domain.Entities;
using Fixy.Domain.Entities.Feedback;

namespace Fixy.Application.Contracts.ExternalServices;

public interface IRatingService
{
    Task UpdateTechnicianRatingAsync(ServiceBooking booking, CustomerFeedback customerFeedback, TechnicianFeedback technicianFeedback);
}
