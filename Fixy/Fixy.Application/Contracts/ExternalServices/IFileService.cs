using Fixy.Application.Common.Models;
using Microsoft.AspNetCore.Http;

namespace Fixy.Application.Contracts.ExternalServices;

public interface IFileService
{
    Task<UploadResultModel> UploadAsync(string Location, IFormFile file);
    Task<string> DeleteAsync(string publicId);
}
