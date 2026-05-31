using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Interfaces;
using Fixy.Domain.SP.TechnicianCashCommissionsOwed.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Payments.Queries.GetTechnicianCashCommissionsOwed;

public sealed class GetTechnicianCashCommissionsOwedQueryHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork) : IRequestHandler<GetTechnicianCashCommissionsOwedQuery, Result<GetTechnicianCashCommissionsOwedResponse>>
{
    public async Task<Result<GetTechnicianCashCommissionsOwedResponse>> Handle(GetTechnicianCashCommissionsOwedQuery request, CancellationToken cancellationToken)
    {
        var currentTechnicianId = await currentUserService.GetCurrentUserId();

        var technician = await unitOfWork.Technicians.GetTableNoTracking().FirstOrDefaultAsync(x => x.Id == currentTechnicianId);
    
        if(technician == null)
        {
            return Errors.TechnicianNotFound;
        }

        var result = await unitOfWork.TechnicianCommissionOwedReadRepository.GetTechnicianCashCommissionsOwedAsync(technician.Id);
        return result;
    }
}
