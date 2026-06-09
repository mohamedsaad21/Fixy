using Fixy.Application.Bases;
using Fixy.Application.Contracts.ExternalServices;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fixy.Application.Features.ServiceRequests.Commands.DeleteServiceRequestImages;

public sealed class DeleteServiceRequestImageByIdCommandHandler(IUnitOfWork unitOfWork, IStorageService fileService, ILogger<DeleteServiceRequestImageByIdCommandHandler> logger) : IRequestHandler<DeleteServiceRequestImageByIdCommand, Result>
{
    public async Task<Result> Handle(DeleteServiceRequestImageByIdCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Attempting to delete service request image. ImageId: {ImageId}", request.ImageId);
        
        var image = await unitOfWork.ServiceRequestImages.GetTableNoTracking().FirstOrDefaultAsync(x => x.Id == request.ImageId);

        if (image == null)
        {
            logger.LogWarning("Service request image deletion failed — image not found. ImageId: {ImageId}", request.ImageId);
            return Errors.ImageNotFound;
        }

        await fileService.DeleteAsync(image.ImageUrl);
        await unitOfWork.ServiceRequestImages.DeleteAsync(image);
        await unitOfWork.SaveChangesAsync();
        logger.LogInformation("Service request image deleted successfully. ImageId: {ImageId}, ImageUrl: {ImageUrl}", image.Id, image.ImageUrl);
        return Result.Success();
    }
}
