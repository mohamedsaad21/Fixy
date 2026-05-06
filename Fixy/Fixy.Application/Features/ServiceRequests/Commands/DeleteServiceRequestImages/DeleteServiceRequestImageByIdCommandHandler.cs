using CloudinaryDotNet.Actions;
using Fixy.Application.Bases;
using Fixy.Application.Contracts.ExternalServices;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.ServiceRequests.Commands.DeleteServiceRequestImages;

public sealed class DeleteServiceRequestImageByIdCommandHandler : IRequestHandler<DeleteServiceRequestImageByIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStorageService _fileService;

    public DeleteServiceRequestImageByIdCommandHandler(IUnitOfWork unitOfWork, IStorageService fileService)
    {
        _unitOfWork = unitOfWork;
        _fileService = fileService;
    }

    public async Task<Result> Handle(DeleteServiceRequestImageByIdCommand request, CancellationToken cancellationToken)
    {
        var image = await _unitOfWork.ServiceRequestImages.GetTableNoTracking().FirstOrDefaultAsync(x => x.Id == request.ImageId);

        if (image == null)
            return Errors.ImageNotFound;

        await _unitOfWork.ServiceRequestImages.DeleteAsync(image);
        await _fileService.DeleteAsync(image.ImageUrl);
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
