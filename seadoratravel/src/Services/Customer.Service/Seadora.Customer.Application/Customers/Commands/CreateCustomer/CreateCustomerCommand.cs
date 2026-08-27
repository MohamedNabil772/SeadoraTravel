using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Common.Tenancy;
using Seadora.Customer.Application.Common.Interfaces;
using CustomerEntity = Seadora.Customer.Domain.Entities.Customer;

namespace Seadora.Customer.Application.Customers.Commands.CreateCustomer;

public record CreateCustomerCommand(
    string FullName,
    string Email,
    string? Phone = null,
    string? Nationality = null,
    string? PassportNumber = null,
    string? Notes = null,
    bool MarketingConsent = false
) : IRequest<Guid>;

public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().Length(2, 100);
        RuleFor(x => x.Email).NotEmpty().Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")
            .WithMessage("Email is not in a valid format.");
    }
}

public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Guid>
{
    private readonly ICustomerDbContext _context;
    private readonly ICurrentBranch _currentBranch;

    public CreateCustomerCommandHandler(ICustomerDbContext context, ICurrentBranch currentBranch)
    {
        _context = context;
        _currentBranch = currentBranch;
    }

    public async Task<Guid> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var branchId = _currentBranch.BranchId;
        var email = CustomerEntity.NormalizeEmail(request.Email);

        // ponytail: explicit pre-check for a friendly 409-ish message; the (BranchId, Email) unique
        // index is still the real guarantee under concurrency.
        if (await _context.Customers.AnyAsync(c => c.BranchId == branchId && c.Email == email, cancellationToken))
        {
            throw new InvalidOperationException("A customer with this email already exists in this branch.");
        }

        var now = DateTime.UtcNow;
        var customer = new CustomerEntity
        {
            Id = Guid.NewGuid(),
            BranchId = branchId,
            FullName = request.FullName.Trim(),
            Email = email,
            Phone = request.Phone,
            Nationality = request.Nationality,
            PassportNumber = request.PassportNumber,
            Notes = request.Notes,
            MarketingConsent = request.MarketingConsent,
            ConsentUpdatedUtc = request.MarketingConsent ? now : null,
            CreatedUtc = now,
            UpdatedUtc = now
        };

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync(cancellationToken);
        return customer.Id;
    }
}
