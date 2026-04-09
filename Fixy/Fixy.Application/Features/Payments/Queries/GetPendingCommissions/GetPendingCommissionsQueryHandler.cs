using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Features.Payments.Queries.GetPendingCommissions.Responses;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fixy.Application.Features.Payments.Queries.GetPendingCommissions;

public class GetPendingCommissionsQueryHandler
        : IRequestHandler<GetPendingCommissionsQuery, Result<GetPendingCommissionsResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<GetPendingCommissionsQueryHandler> _logger;

    public GetPendingCommissionsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, ILogger<GetPendingCommissionsQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<Result<GetPendingCommissionsResponse>> Handle(GetPendingCommissionsQuery request, CancellationToken cancellationToken)
    {
        // 1. Get current user (technician)
        var technician = await _currentUserService.GetCurrentUserAsync();

        // 2. Get pending commissions
        var commissions = await _unitOfWork.TechnicianCommissionsOwed.GetTableNoTracking()
            .Where(c => c.TechnicianId == technician.Id && !c.IsPaid)
            .Include(c => c.Booking)
                .ThenInclude(b => b.ServiceRequest)
                    .ThenInclude(sr => sr.Customer)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);

        // 3. Calculate totals
        var totalOwed = commissions.Sum(c => c.AmountOwed);

        _logger.LogInformation($"Technician {technician.Id} has {commissions.Count} pending commissions totaling {totalOwed:C}");

        // 4. Build response
        var response = new GetPendingCommissionsResponse
        {
            TotalOwed = totalOwed,
            CommissionCount = commissions.Count,
            Commissions = commissions.Select(c => new CommissionItem
            {
                Id = c.Id,
                BookingId = c.BookingId,
                CustomerName = c.Booking.ServiceRequest.Customer.FullName,
                ServiceDescription = c.Booking.ServiceRequest.Description,
                AmountOwed = c.AmountOwed,
                CreatedAt = c.CreatedAt
            }).ToList()
        };

        return response;
    }
}
