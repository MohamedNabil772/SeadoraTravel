using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Common.Tenancy;
using Seadora.Customer.Application.Common.Interfaces;
using Seadora.Customer.Application.Common.Models;
using Seadora.Customer.Application.DTOs;

namespace Seadora.Customer.Application.Customers.Queries.GetCustomers;

public record GetCustomersQuery(string? Search = null, int PageNumber = 1, int PageSize = 20)
    : IRequest<PagedResult<CustomerDto>>;

public class GetCustomersQueryHandler : IRequestHandler<GetCustomersQuery, PagedResult<CustomerDto>>
{
    private readonly ICustomerDbContext _context;
    private readonly ICurrentBranch _currentBranch;

    public GetCustomersQueryHandler(ICustomerDbContext context, ICurrentBranch currentBranch)
    {
        _context = context;
        _currentBranch = currentBranch;
    }

    public async Task<PagedResult<CustomerDto>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
    {
        var branchId = _currentBranch.BranchId;
        var query = _context.Customers.AsNoTracking().Where(c => c.BranchId == branchId);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            // ponytail: ToLower().Contains beats a provider-specific ILIKE - same result, and it also
            // runs under the InMemory provider the tests use. Swap to ILike if search ever gets slow.
            var search = request.Search.Trim().ToLower();
            query = query.Where(c => c.FullName.ToLower().Contains(search) || c.Email.Contains(search));
        }

        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize is < 1 or > 100 ? 20 : request.PageSize;

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(c => c.CreatedUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new CustomerDto(c.Id, c.FullName, c.Email, c.Phone, c.Nationality, c.MarketingConsent, c.CreatedUtc))
            .ToListAsync(cancellationToken);

        return new PagedResult<CustomerDto>
        {
            Items = items,
            TotalCount = total,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}
