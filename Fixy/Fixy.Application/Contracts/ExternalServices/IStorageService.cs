using Microsoft.AspNetCore.Http;

namespace Fixy.Application.Contracts.ExternalServices;

public interface IStorageService
{
    Task<string> UploadAsync(IFormFile file);
    Task DeleteAsync(string url);
}
