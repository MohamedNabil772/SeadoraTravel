using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Common.Tenancy;
using Seadora.Customer.Application.Common.Interfaces;

namespace Seadora.Customer.Application.Customers.Commands.UpdateMarketingConsent;

public record UpdateMarketingConsentCommand(Guid Id, bool Consent) : IRequest<Unit>;

public class UpdateMarketingConsentCommandHandler : IRequestHandler<UpdateMarketingConsentCommand, Unit>
{
    private readonly ICustomerDbContext _context;
    private readonly ICurrentBranch _currentBranch;

    public UpdateMarketingConsentCommandHandler(ICustomerDbContext context, ICurrentBranch currentBranch)
    {
        _context = context;
        _currentBranch = currentBranch;
    }

    public async Task<Unit> Handle(UpdateMarketingConsentCommand request, CancellationToken cancellationToken)
    {
        var branchId = _currentBranch.BranchId;
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == request.Id && c.BranchId == branchId, cancellationToken)
            ?? throw new KeyNotFoundException($"Customer {request.Id} was not found.");

        var now = DateTime.UtcNow;
        customer.MarketingConsent = request.Consent;
        customer.ConsentUpdatedUtc = now;
        customer.UpdatedUtc = now;

        await _context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
