using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Common.Tenancy;
using Seadora.Customer.Application.Common.Interfaces;
using Seadora.Customer.Application.DTOs;

namespace Seadora.Customer.Application.Customers.Queries.GetCustomerById;

public record GetCustomerByIdQuery(Guid Id) : IRequest<CustomerDetailDto?>;

public class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, CustomerDetailDto?>
{
    private readonly ICustomerDbContext _context;
    private readonly ICurrentBranch _currentBranch;

    public GetCustomerByIdQueryHandler(ICustomerDbContext context, ICurrentBranch currentBranch)
    {
        _context = context;
        _currentBranch = currentBranch;
    }

    public async Task<CustomerDetailDto?> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        var branchId = _currentBranch.BranchId;
        var customer = await _context.Customers
            .AsNoTracking()
            .Include(c => c.Documents)
            .FirstOrDefaultAsync(c => c.Id == request.Id && c.BranchId == branchId, cancellationToken);

        if (customer is null) return null;

        return new CustomerDetailDto(
            customer.Id,
            customer.FullName,
            customer.Email,
            customer.Phone,
            customer.Nationality,
            customer.PassportNumber,
            customer.Notes,
            customer.MarketingConsent,
            customer.ConsentUpdatedUtc,
            customer.CreatedUtc,
            customer.UpdatedUtc,
            customer.Documents
                .OrderByDescending(d => d.UploadedUtc)
                .Select(d => new CustomerDocumentDto(d.Id, d.CustomerId, d.DocumentType, d.FileRef, d.FileName, d.UploadedUtc, d.RetentionUntilUtc))
                .ToList());
    }
}
