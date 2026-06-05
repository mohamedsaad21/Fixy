using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Interfaces;
using Fixy.Domain.SP.TechnicianCashCommissionsOwed.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fixy.Application.Features.Payments.Queries.GetTechnicianCashCommissionsOwed;

public sealed class GetTechnicianCashCommissionsOwedQueryHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork, ILogger<GetTechnicianCashCommissionsOwedQueryHandler> logger) : IRequestHandler<GetTechnicianCashCommissionsOwedQuery, Result<GetTechnicianCashCommissionsOwedResponse>>
{
    public async Task<Result<GetTechnicianCashCommissionsOwedResponse>> Handle(GetTechnicianCashCommissionsOwedQuery request, CancellationToken cancellationToken)
    {
        var currentTechnicianId = await currentUserService.GetCurrentUserId();

        logger.LogInformation("Fetching cash commissions owed. TechnicianId: {TechnicianId}", currentTechnicianId);

        var technician = await unitOfWork.Technicians.GetTableNoTracking().FirstOrDefaultAsync(x => x.Id == currentTechnicianId);
    
        if(technician == null)
        {
            logger.LogWarning("Cash commissions fetch failed — technician not found. TechnicianId: {TechnicianId}", currentTechnicianId);
            return Errors.TechnicianNotFound;
        }

        var result = await unitOfWork.TechnicianCommissionOwedReadRepository.GetTechnicianCashCommissionsOwedAsync(technician.Id);
        logger.LogInformation("Cash commissions owed fetched successfully. TechnicianId: {TechnicianId}, TotalOwed: {TotalOwed}, PendingJobsCount: {PendingJobsCount}", technician.Id, result.TotalAmountOwed, result.BookingCount);
        return result;
    }
}
