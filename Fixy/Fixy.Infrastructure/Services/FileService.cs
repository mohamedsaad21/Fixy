using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Fixy.Application.Abstracts;
using Fixy.Application.Common.Models;
using Microsoft.AspNetCore.Http;

namespace Fixy.Infrastructure.Services;

public class FileService : IFileService
{
    private readonly Cloudinary _cloudinary;

    public FileService(Cloudinary cloudinary)
    {
        _cloudinary = cloudinary;
    }

    public async Task<UploadResultModel> UploadAsync(string Location, IFormFile file)
    {
        if (file is null || file.Length == 0)
            return new UploadResultModel { Message = "NoFile" };

        try
        {
            await using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = Location,
                UseFilename = true,
                UniqueFilename = true
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            return new UploadResultModel
            {
                Url = uploadResult.SecureUrl?.ToString(),
                PublicId = uploadResult.PublicId
            };
        }
        catch
        {
            return new UploadResultModel { Message = "FailedToUploadImage" };
        }
    }

    public async Task<string> DeleteAsync(string publicId)
    {
        var deletionParams = new DeletionParams(publicId)
        {
            ResourceType = ResourceType.Image
        };        
        var deletionResult = _cloudinary.Destroy(deletionParams);
        return deletionResult.Result;
    }
}
