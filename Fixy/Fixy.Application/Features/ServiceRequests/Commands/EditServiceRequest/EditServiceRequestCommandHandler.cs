using Fixy.Application.Bases;
using Fixy.Application.Contracts.ExternalServices;
using Fixy.Application.Mapping.ServiceRequests.Commands;
using Fixy.Domain.Entities;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.ServiceRequests.Commands.EditServiceRequest;

public sealed class EditServiceRequestCommandHandler : IRequestHandler<EditServiceRequestCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileService _fileService;
    public EditServiceRequestCommandHandler(IUnitOfWork unitOfWork, IFileService fileService)
    {
        _unitOfWork = unitOfWork;
        _fileService = fileService;
    }

    public async Task<Result> Handle(EditServiceRequestCommand request, CancellationToken cancellationToken)
    {
        var serviceRequest = await _unitOfWork.ServiceRequests.GetTableAsTracking()
            .Include(x => x.ServiceCategories).FirstOrDefaultAsync(x => x.Id == request.Id);

        if (serviceRequest == null)
            return Errors.ServiceRequestNotFound;

        serviceRequest = request.ToServiceRequestDomain(serviceRequest);
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
