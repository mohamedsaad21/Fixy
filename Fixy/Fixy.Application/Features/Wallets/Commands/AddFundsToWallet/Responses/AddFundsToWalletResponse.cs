namespace Fixy.Application.Features.Wallets.Commands.AddFundsToWallet.Responses;

public class AddFundsToWalletResponse
{
    public string PaymentUrl { get; set; }
    public string StripeSessionId { get; set; }
    public string MerchantOrderId { get; set; }
    public decimal Amount { get; set; }
}
