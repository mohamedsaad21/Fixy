namespace Fixy.Application.Common.Models;

public class UploadResultModel
{
    public bool IsSuccess => !string.IsNullOrWhiteSpace(Url) && !string.IsNullOrWhiteSpace(PublicId);
    public string? Url { get; set; }
    public string? PublicId { get; set; }
    public string? Message { get; set; }
}
