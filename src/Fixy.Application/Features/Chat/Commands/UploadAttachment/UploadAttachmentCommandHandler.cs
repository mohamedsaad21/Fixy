using Fixy.Application.Bases;
using Fixy.Application.Contracts.ExternalServices;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fixy.Application.Features.Chat.Commands.UploadAttachment;

public sealed class UploadAttachmentCommandHandler(IStorageService storageService, ILogger<UploadAttachmentCommandHandler>logger) : IRequestHandler<UploadAttachmentCommand, Result<string>>
{
    public async Task<Result<string>> Handle(UploadAttachmentCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Uploading chat attachment. FileName: {FileName}, FileSize: {FileSize}", request.Attachment.FileName, request.Attachment.Length);
        var url = await storageService.UploadAsync(request.Attachment);
        logger.LogInformation("Chat attachment uploaded successfully. FileName: {FileName}, Url: {Url}", request.Attachment.FileName, url);
        return url;
    }
}
