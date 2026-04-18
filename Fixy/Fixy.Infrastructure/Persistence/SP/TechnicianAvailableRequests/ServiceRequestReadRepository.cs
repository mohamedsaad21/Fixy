using Fixy.Domain.SP.TechnicianAvailableRequests;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Infrastructure.Persistence.SP.TechnicianAvailableRequests;

public class ServiceRequestReadRepository : IServiceRequestReadRepository
{
    private readonly FixyDbContext _dbContext;
    public ServiceRequestReadRepository(FixyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<ServiceRequestSpResult>> GetTechnicianAvailableServiceRequest(int PageNumber, int PageSize, Guid TechnicianId, double Latitude, double Longitude, Guid CategoryId)
    {
        var parameters = new SqlParameter[]
        {
            new SqlParameter("@TechnicianId", TechnicianId),
            new SqlParameter("@Latitude", Latitude),
            new SqlParameter("@Longitude", Longitude),
            new SqlParameter("@CategoryId", CategoryId),
            new SqlParameter("@PageNumber", PageNumber),
            new SqlParameter("@PageSize", PageSize),
        };
        var result = await _dbContext.Set<ServiceRequestSpResult>().FromSqlRaw(@"
            EXEC [dbo].[GetTechnicianAvailableRequests]
            @TechnicianId,
            @Latitude,
            @Longitude,
            @CategoryId,
            @PageNumber,
            @PageSize",
            parameters)
            .AsNoTracking()
            .ToListAsync();
        return result;
    }
}
