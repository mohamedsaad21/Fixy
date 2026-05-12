namespace Fixy.Application.Common.DTOs.Payment;

public class PaymobCallbackDto
{
    public PaymobTransaction obj { get; set; }
    public string type { get; set; }
    public string hmac { get; set; }
}
