namespace Fixy.Domain.SP.TechnicianAvailableRequests;

public interface IServiceRequestReadRepository
{
    Task<List<ServiceRequestSpResult>> GetTechnicianAvailableServiceRequest(int PageNumber, int PageSize, Guid TechnicianId, double Latitude, double Longitude, Guid CategoryId);
}
