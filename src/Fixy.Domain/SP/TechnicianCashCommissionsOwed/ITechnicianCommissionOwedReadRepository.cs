using Fixy.Domain.SP.TechnicianCashCommissionsOwed.Responses;

namespace Fixy.Domain.SP.TechnicianCashCommissionsOwed;

public interface ITechnicianCommissionOwedReadRepository
{
    Task<GetTechnicianCashCommissionsOwedResponse> GetTechnicianCashCommissionsOwedAsync(Guid TechnicianId);
}
