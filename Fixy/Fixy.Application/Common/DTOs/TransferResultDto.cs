namespace Fixy.Application.Common.DTOs;

public class TransferResultDto
{
    public bool Success { get; set; }
    public string TransferId { get; set; } = string.Empty;
    public decimal AmountTransferred { get; set; }
    public string Message { get; set; } = string.Empty;
}
