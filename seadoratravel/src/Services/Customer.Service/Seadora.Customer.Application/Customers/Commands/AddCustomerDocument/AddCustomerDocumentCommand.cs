using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Common.Tenancy;
using Seadora.Customer.Application.Common.Interfaces;
using Seadora.Customer.Domain.Entities;

namespace Seadora.Customer.Application.Customers.Commands.AddCustomerDocument;

public record AddCustomerDocumentCommand(
    Guid CustomerId,
    string DocumentType,
    string FileRef,
    string FileName,
    DateTime? RetentionUntilUtc = null
) : IRequest<Guid>;

public class AddCustomerDocumentCommandValidator : AbstractValidator<AddCustomerDocumentCommand>
{
    public AddCustomerDocumentCommandValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.DocumentType).NotEmpty().MaximumLength(50);
        RuleFor(x => x.FileRef).NotEmpty().MaximumLength(500);
        RuleFor(x => x.FileName).NotEmpty().MaximumLength(260);
    }
}

public class AddCustomerDocumentCommandHandler : IRequestHandler<AddCustomerDocumentCommand, Guid>
{
    private readonly ICustomerDbContext _context;
    private readonly ICurrentBranch _currentBranch;

    public AddCustomerDocumentCommandHandler(ICustomerDbContext context, ICurrentBranch currentBranch)
    {
        _context = context;
        _currentBranch = currentBranch;
    }

    public async Task<Guid> Handle(AddCustomerDocumentCommand request, CancellationToken cancellationToken)
    {
        var branchId = _currentBranch.BranchId;
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == request.CustomerId && c.BranchId == branchId, cancellationToken)
            ?? throw new KeyNotFoundException($"Customer {request.CustomerId} was not found.");

        var document = new CustomerDocument
        {
            Id = Guid.NewGuid(),
            CustomerId = customer.Id,
            DocumentType = request.DocumentType,
            FileRef = request.FileRef,
            FileName = request.FileName,
            UploadedUtc = DateTime.UtcNow,
            RetentionUntilUtc = request.RetentionUntilUtc
        };

        // ponytail: add through the DbSet, not customer.Documents - the Id is client-assigned, so a
        // nav-added child is detected as Modified and EF would UPDATE a row that does not exist yet.
        _context.CustomerDocuments.Add(document);
        customer.UpdatedUtc = document.UploadedUtc;

        await _context.SaveChangesAsync(cancellationToken);
        return document.Id;
    }
}
