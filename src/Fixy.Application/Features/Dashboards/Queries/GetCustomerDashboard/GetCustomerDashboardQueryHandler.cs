using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Dashboards.Queries.GetCustomerDashboard;

public sealed class GetCustomerDashboardQueryHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork) : IRequestHandler<GetCustomerDashboardQuery, Result<GetCustomerDashboardResponse>>
{
    public async Task<Result<GetCustomerDashboardResponse>> Handle(GetCustomerDashboardQuery request, CancellationToken cancellationToken)
    {
        var currentCustomerId = await currentUserService.GetCurrentUserId();

        var customer = await unitOfWork.Customers.GetTableNoTracking().FirstOrDefaultAsync(x => x.Id == currentCustomerId);

        if (customer == null)
            return Errors.CustomerNotFound;

        var serviceRequestsQuery = unitOfWork.ServiceRequests.GetTableNoTracking().Where(x => x.CustomerId == customer.Id).AsQueryable();

        var pendingServiceRequestsCount = await serviceRequestsQuery.CountAsync(x => x.Status == ServiceRequestStatus.Pending);

        var cancelledServiceRequestsCount = await serviceRequestsQuery.CountAsync(x => x.Status == ServiceRequestStatus.Cancelled);

        var bookingsQuery = unitOfWork.Bookings.GetTableNoTracking()
            .Where(x => x.ServiceRequest.CustomerId == customer.Id).AsQueryable();

        var inProgressBookingsCount =
            await bookingsQuery.CountAsync(x => x.Status == ServiceBookingStatus.InProgress);

        var completedBookingsCount =
            await bookingsQuery.CountAsync(x => x.Status == ServiceBookingStatus.CustomerCompleted);

        var cancelledBookingsCount =
            await bookingsQuery.CountAsync(x => x.Status == ServiceBookingStatus.CancelledByCustomer);

        var result = new GetCustomerDashboardResponse
        {
            PendingServiceRequestsCount = pendingServiceRequestsCount,
            CancelledServiceRequestsCount = cancelledServiceRequestsCount,
            InProgressBookingsCount = inProgressBookingsCount,
            CompletedBookingsCount = completedBookingsCount,
            CancelledBookingsCount = cancelledBookingsCount,
            CancellationRate = customer.CancellationRate
        };
        return result;

    }
}
