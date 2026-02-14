using Fixy.Application.Common.DTOs;
using Stripe;

namespace Fixy.Application.Abstracts;

public interface IStripeTransferService
{
    Task<TransferResultDto> TransferToTechnicianAsync(Guid bookingId);
    Task<Transfer> GetTransferAsync(string transferId);
    Task<bool> ReverseTransferAsync(string transferId);
}
