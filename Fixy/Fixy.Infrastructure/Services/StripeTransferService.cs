using Fixy.Application.Abstracts;
using Fixy.Application.Common.DTOs;
using Fixy.Domain.Entities;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace Fixy.Infrastructure.Services;

public class StripeTransferService : IStripeTransferService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly TransferService _transferService;
    private readonly TransferReversalService _reversalService;

    public StripeTransferService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _transferService = new TransferService();
        _reversalService = new TransferReversalService();
    }

    /// <summary>
    /// Transfer funds from platform to technician (ESCROW RELEASE)
    /// This is called when service is confirmed as completed
    /// </summary>
    public async Task<TransferResultDto> TransferToTechnicianAsync(Guid bookingId)
    {
        // Get booking with related data
        var booking = await _unitOfWork.Bookings.GetTableAsTracking()
            .Include(b => b.Payment)
            .Include(b => b.Technician)
            .FirstOrDefaultAsync(b => b.Id == bookingId);

        if (booking == null)
            return new TransferResultDto { Success = false, Message = "Booking not found" };

        if (booking.Payment == null)
            return new TransferResultDto { Success = false, Message = "Payment not found" };

        // Verify payment was successful
        if (booking.Payment.Status != "succeeded")
            return new TransferResultDto { Success = false, Message = "Payment not succeeded" };

        // Check if transfer already exists
        var existingTransfer = await _unitOfWork.TechnicianTransfers.GetTableNoTracking()
            .FirstOrDefaultAsync(t => t.ServiceBookingId == bookingId);

        if (existingTransfer != null)
            return new TransferResultDto
            {
                Success = true,
                TransferId = existingTransfer.StripeTransferId,
                AmountTransferred = existingTransfer.Amount,
                Message = "Transfer already completed"
            };

        // Get technician's Stripe account
        var technicianAccount = await _unitOfWork.TechnicianStripeAccounts.GetTableAsTracking()
            .FirstOrDefaultAsync(a => a.TechnicianId == booking.TechnicianId);

        if (technicianAccount == null)
            return new TransferResultDto { Success = false, Message = "Technician Stripe account not found" };

        if (!technicianAccount.PayoutsEnabled)
            return new TransferResultDto { Success = false, Message = "Technician payouts not enabled" };

        // Calculate transfer amount (90% to technician)
        var transferAmount = booking.Payment.TechnicianAmount;
        var amountInCents = (long)(transferAmount * 100);

        // Create transfer to technician's Stripe account
        var options = new TransferCreateOptions
        {
            Amount = amountInCents,
            Currency = "usd",

            // CRITICAL: Destination is the TECHNICIAN's connected account
            Destination = technicianAccount.StripeAccountId,

            // Link to original payment (important for refunds/disputes)
            SourceTransaction = booking.Payment.StripePaymentIntentId,

            // Metadata
            Metadata = new Dictionary<string, string>
                {
                    { "booking_id", bookingId.ToString() },
                    { "technician_id", booking.TechnicianId.ToString() },
                    { "payment_id", booking.Payment.Id.ToString() }
                },

            Description = $"Payment for service booking #{bookingId}"
        };

        Transfer transfer;
        try
        {
            transfer = await _transferService.CreateAsync(options);
        }
        catch (StripeException ex)
        {
            return new TransferResultDto
            {
                Success = false,
                Message = $"Stripe error: {ex.Message}"
            };
        }

        // Store transfer record
        var transferRecord = new TechnicianTransfer
        {
            ServiceBookingId = bookingId,
            TechnicianId = booking.TechnicianId,
            StripeTransferId = transfer.Id,
            Amount = transferAmount,
            Currency = "usd",
            Status = TransferStatus.Pending, // Will be updated by webhook
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.TechnicianTransfers.AddAsync(transferRecord);

        // Update booking status
        booking.Status = ServiceBookingStatus.FundsTransferred;

        await _unitOfWork.SaveChangesAsync();

        return new TransferResultDto
        {
            Success = true,
            TransferId = transfer.Id,
            AmountTransferred = transferAmount,
            Message = "Transfer successful"
        };
    }

    /// <summary>
    /// Get transfer details from Stripe
    /// </summary>
    public async Task<Transfer> GetTransferAsync(string transferId)
    {
        return await _transferService.GetAsync(transferId);
    }

    /// <summary>
    /// Reverse a transfer (for disputes where technician didn't complete service)
    /// </summary>
    public async Task<bool> ReverseTransferAsync(string transferId)
    {
        try
        {
            var options = new TransferReversalCreateOptions
            {
                Description = "Service not completed - refunding customer"
            };

            await _reversalService.CreateAsync(transferId, options);

            // Update transfer status
            var transfer = await _unitOfWork.TechnicianTransfers.GetTableAsTracking()
                .FirstOrDefaultAsync(t => t.StripeTransferId == transferId);

            if (transfer != null)
            {
                transfer.Status = TransferStatus.Reversed;
                await _unitOfWork.SaveChangesAsync();
            }

            return true;
        }
        catch (StripeException)
        {
            return false;
        }
    }
}