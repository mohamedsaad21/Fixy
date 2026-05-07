using Fixy.Application.Bases;
using Fixy.Application.Contracts.ExternalServices;
using Fixy.Domain.Entities;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.ServiceRequests.Commands.AddServiceRequestImages;

public sealed class AddServiceRequestImagesCommandHandler : IRequestHandler<AddServiceRequestImagesCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStorageService _fileService;
    public AddServiceRequestImagesCommandHandler(IUnitOfWork unitOfWork, IStorageService fileService)
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
        var UploadResults = new List<string>();
        try {
            foreach (var image in request.Images)
            {
                var url = await _fileService.UploadAsync(image);
                serviceRequest.ServiceRequestImages.Add(new ServiceRequestImage
                {
                    ImageUrl = url,
                });
                UploadResults.Add(url);
            }
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception)
        {
            foreach (var url in UploadResults)
            {
                await _fileService.DeleteAsync(url);
            }
            return Errors.RequestInsertionFailed;
        }
    }
}
