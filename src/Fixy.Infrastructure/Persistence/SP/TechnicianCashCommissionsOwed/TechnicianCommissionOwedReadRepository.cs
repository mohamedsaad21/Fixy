using Dapper;
using Fixy.Domain.SP.TechnicianCashCommissionsOwed;
using Fixy.Domain.SP.TechnicianCashCommissionsOwed.Responses;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Fixy.Infrastructure.Persistence.SP.TechnicianCashCommissionsOwed;

internal class TechnicianCommissionOwedReadRepository : ITechnicianCommissionOwedReadRepository
{
    private readonly FixyDbContext _dbContext;
    public TechnicianCommissionOwedReadRepository(FixyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GetTechnicianCashCommissionsOwedResponse> GetTechnicianCashCommissionsOwedAsync(Guid TechnicianId)
    {
        var connection = _dbContext.Database.GetDbConnection();

        using var multi = await connection.QueryMultipleAsync(
            "[dbo].[GetTechnicianCashCommissionsOwed]",
            new { TechnicianId = TechnicianId },
            commandType: CommandType.StoredProcedure
        );

        var response = new GetTechnicianCashCommissionsOwedResponse();

        var totals = await multi.ReadFirstAsync<dynamic>();
        response.TotalAmountOwed = totals.TotalAmount;
        response.BookingCount = totals.BookingCount;

        var bookingsList = await multi.ReadAsync<CashCommissionItem>();
        response.Bookings = bookingsList.ToList();

        return response;
    }
}
