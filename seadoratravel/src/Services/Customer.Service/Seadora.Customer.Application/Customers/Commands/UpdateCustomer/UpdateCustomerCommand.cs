using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Common.Tenancy;
using Seadora.Customer.Application.Common.Interfaces;
using CustomerEntity = Seadora.Customer.Domain.Entities.Customer;

namespace Seadora.Customer.Application.Customers.Commands.UpdateCustomer;

public record UpdateCustomerCommand(
    Guid Id,
    string FullName,
    string Email,
    string? Phone = null,
    string? Nationality = null,
    string? PassportNumber = null,
    string? Notes = null
) : IRequest<Unit>;

public class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
{
    public UpdateCustomerCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.FullName).NotEmpty().Length(2, 100);
        RuleFor(x => x.Email).NotEmpty().Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")
            .WithMessage("Email is not in a valid format.");
    }
}

public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, Unit>
{
    private readonly ICustomerDbContext _context;
    private readonly ICurrentBranch _currentBranch;

    public UpdateCustomerCommandHandler(ICustomerDbContext context, ICurrentBranch currentBranch)
    {
        _context = context;
        _currentBranch = currentBranch;
    }

    public async Task<Unit> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        var branchId = _currentBranch.BranchId;
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == request.Id && c.BranchId == branchId, cancellationToken)
            ?? throw new KeyNotFoundException($"Customer {request.Id} was not found.");

        var email = CustomerEntity.NormalizeEmail(request.Email);
        if (email != customer.Email &&
            await _context.Customers.AnyAsync(c => c.BranchId == branchId && c.Email == email, cancellationToken))
        {
            throw new InvalidOperationException("A customer with this email already exists in this branch.");
        }

        customer.FullName = request.FullName.Trim();
        customer.Email = email;
        customer.Phone = request.Phone;
        customer.Nationality = request.Nationality;
        customer.PassportNumber = request.PassportNumber;
        customer.Notes = request.Notes;
        customer.UpdatedUtc = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
