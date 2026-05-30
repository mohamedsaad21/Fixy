using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Fixy.Application.Contracts.ExternalServices;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Fixy.Infrastructure.ExternalServices;

public class BlobStorageService : IStorageService
{
    private readonly BlobContainerClient _containerClient;

    public BlobStorageService(IConfiguration configuration)
    {
        var connectionString = configuration["Azure:Storage:ConnectionString"];
        var containerName = configuration["Azure:Storage:ContainerName"];

        if (string.IsNullOrEmpty(connectionString))
            throw new ArgumentNullException(nameof(connectionString), "Azure Storage connection string is not configured");

        if (string.IsNullOrEmpty(containerName))
            throw new ArgumentNullException(nameof(containerName), "Azure Storage container name is not configured");

        _containerClient = new BlobContainerClient(connectionString, containerName);
        _containerClient.CreateIfNotExists();
    }

    public async Task<string> UploadAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File is empty or null");

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var fileName = $"{Guid.NewGuid()}{extension}";

        var blobClient = _containerClient.GetBlobClient(fileName);

        var blobHttpHeaders = new BlobHttpHeaders
        {
            ContentType = file.ContentType
        };

        using var stream = file.OpenReadStream();
        await blobClient.UploadAsync(stream, new BlobUploadOptions
        {
            HttpHeaders = blobHttpHeaders
        });

        return blobClient.Uri.ToString();
    }

    public async Task DeleteAsync(string url)
    {
        if (string.IsNullOrEmpty(url))
            throw new ArgumentNullException(nameof(url), "URL cannot be empty");

        var uri = new Uri(url);

        if (!uri.ToString().StartsWith(_containerClient.Uri.ToString()))
            throw new ArgumentException("URL does not belong to this container");

        var fileName = Path.GetFileName(uri.LocalPath);
        var blobClient = _containerClient.GetBlobClient(fileName);
        await blobClient.DeleteIfExistsAsync();
    }
}