namespace Fixy.Application.Common.DTOs.Payment;

public class PaymobOrder
{
    public int id { get; set; }
    public string merchant_Order_Id { get; set; }
    public decimal amount_Cents { get; set; }
}
