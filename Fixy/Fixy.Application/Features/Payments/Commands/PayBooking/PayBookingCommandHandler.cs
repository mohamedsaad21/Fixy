using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Features.Payments.Commands.PayBooking.Responses;
using Fixy.Domain.Constants;
using Fixy.Domain.Entities.Payments;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Fixy.Application.Features.Payments.Commands.PayBooking;

public sealed class PayBookingCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService) : IRequestHandler<PayBookingCommand, Result<PayBookingResponse>>
{
    public async Task<Result<PayBookingResponse>> Handle(PayBookingCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var currentCustomer = await currentUserService.GetCurrentUserAsync();

            var booking = await unitOfWork.Bookings.GetTableAsTracking().FirstOrDefaultAsync(x => x.Id == request.BookingId);
            if (booking == null)
                return Errors.BookingNotFound;

            if (booking.Status != ServiceBookingStatus.AwaitingPayment)
            {
                Log.Warning($"Booking {request.BookingId} is not in WaitingPayment status");
                return Errors.BookingNotReadyForPayment;
            }

            var existingPayment = await unitOfWork.Payments.GetTableNoTracking()
                                    .FirstOrDefaultAsync(p => p.ServiceBookingId == request.BookingId && p.Status == PaymentStatus.Success);

            if (existingPayment != null)
                return Errors.PaymentAlreadyCompleted;

            var totalAmount = booking.AgreedPrice;
            var platformCommission = totalAmount * WalletConstants.CommissionRate;
            var technicianAmount = totalAmount - platformCommission;

            var payment = new Payment
            {
                ServiceBookingId = request.BookingId,
                UserId = currentCustomer.Id,
                TotalAmount = totalAmount,
                TechnicianAmount = totalAmount * 0.85m,
                PlatformCommission = totalAmount * 0.15m,
                Method = request.PaymentMethod,
                Status = PaymentStatus.Pending,
            };

            var response = new PayBookingResponse
            {
                PaymentMethod = request.PaymentMethod,
                Amount = totalAmount,
            };

            var technicianWallet = await unitOfWork.Wallets.GetTableAsTracking().FirstOrDefaultAsync(x => x.ApplicationUserId == booking.TechnicianId);

            if (technicianWallet == null)
                return Errors.TechnicianWalletNotFound;

            if (request.PaymentMethod == PaymentMethod.Wallet)
            {
                var customerWallet = await unitOfWork.Wallets.GetTableAsTracking().FirstOrDefaultAsync(x => x.ApplicationUserId == currentCustomer.Id);

                if (customerWallet == null)
                    return Errors.CustomerWalletNotFound;

                if (customerWallet.Balance < booking.AgreedPrice)
                return Errors.InsufficientWalletBalance;

                var transaction = new WalletTransaction
                {
                    WalletId = customerWallet.Id,
                    Amount = booking.AgreedPrice,
                    BalanceBefore = customerWallet.Balance,
                    BalanceAfter = customerWallet.Balance - booking.AgreedPrice,
                    Type = WalletTransactionType.BookingPayment,
                    Status = WalletTransactionStatus.Success,
                    Description = $"Payment for booking #{booking.Id} - Amount: {booking.AgreedPrice} EGP",
                };
                await unitOfWork.WalletTransactions.AddAsync(transaction);

                customerWallet.Balance -= transaction.Amount;
                customerWallet.UpdatedAt = DateTime.UtcNow;
                
                technicianWallet.Balance += booking.AgreedPrice * WalletConstants.TechnicianRate;
                technicianWallet.UpdatedAt = DateTime.UtcNow;
                payment.Status = PaymentStatus.Success;
                payment.PaidAt = DateTime.UtcNow;
                booking.Status = ServiceBookingStatus.Completed;
            }
            else if (request.PaymentMethod == PaymentMethod.Cash)
            {
                // When Technician Confirms Cash Receipt
                //technicianWallet.Balance -= booking.AgreedPrice * WalletConstants.CommissionRate;
                //technicianWallet.UpdatedAt = DateTime.UtcNow;
            }

            await unitOfWork.Payments.AddAsync(payment);

            await unitOfWork.SaveChangesAsync();

            Log.Information($"Payment {payment.Id} created successfully for booking {request.BookingId}");

            return response;
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Error creating payment for booking {request.BookingId}");
            return Errors.PaymentCreationFailed;
        }
    }
}
