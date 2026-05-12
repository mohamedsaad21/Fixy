namespace Fixy.Application.Common.DTOs.Payment;

public class PaymobTransaction
{
    public int id { get; set; }
    public bool success { get; set; }
    public decimal amount_Cents { get; set; }
    public string currency { get; set; }
    public PaymobOrder order { get; set; }
    public string created_At { get; set; }
}
