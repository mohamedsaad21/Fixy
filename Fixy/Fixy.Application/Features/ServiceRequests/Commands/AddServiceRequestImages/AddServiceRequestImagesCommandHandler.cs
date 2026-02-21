using Fixy.Application.Abstracts;
using Fixy.Application.Bases;
using Fixy.Application.Common.Models;
using Fixy.Domain.Entities;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.ServiceRequests.Commands.AddServiceRequestImages;

public sealed class AddServiceRequestImagesCommandHandler : IRequestHandler<AddServiceRequestImagesCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileService _fileService;
    public AddServiceRequestImagesCommandHandler(IUnitOfWork unitOfWork, IFileService fileService)
    {
        _unitOfWork = unitOfWork;
        _fileService = fileService;
    }

    public async Task<Result> Handle(AddServiceRequestImagesCommand request, CancellationToken cancellationToken)
    {
        var serviceRequest = await _unitOfWork.ServiceRequests.GetTableAsTracking().FirstOrDefaultAsync(x => x.Id == request.ServiceRequestId);

        if (serviceRequest == null)
            return Errors.ServiceRequestNotFound;

        // Upload service request images
        var UploadResults = new List<UploadResultModel>();
        try {
            foreach (var image in request.Images)
            {
                var uploadResult = await _fileService.UploadAsync("ServiceRequest/temp", image);
                serviceRequest.ServiceRequestImages.Add(new ServiceRequestImage
                {
                    ImageUrl = uploadResult.Url,
                    ImagePublicId = uploadResult.PublicId
                });
                UploadResults.Add(uploadResult);
            }
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception)
        {
            foreach (var uploadResult in UploadResults)
            {
                await _fileService.DeleteAsync(uploadResult.PublicId);
            }
            return Errors.RequestInsertionFailed;
        }
    }
}
