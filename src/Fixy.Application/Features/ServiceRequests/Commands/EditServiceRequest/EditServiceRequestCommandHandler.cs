using Fixy.Application.Bases;
using Fixy.Application.Contracts.ExternalServices;
using Fixy.Application.Mapping.ServiceRequests.Commands;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.ServiceRequests.Commands.EditServiceRequest;

public sealed class EditServiceRequestCommandHandler : IRequestHandler<EditServiceRequestCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStorageService _fileService;
    public EditServiceRequestCommandHandler(IUnitOfWork unitOfWork, IStorageService fileService)
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

        // Load only the categories that were submitted
        var newCategories = await _unitOfWork.ServiceCategories
            .GetTableAsTracking()
            .Where(c => request.ServiceCategories.Contains(c.Id))
            .ToListAsync(cancellationToken);

        serviceRequest = request.ToServiceRequestDomain(serviceRequest, newCategories);
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
