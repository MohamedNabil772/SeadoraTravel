using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Seadora.Contracts.Identity;
using Seadora.Customer.Domain.Entities;
using Seadora.Customer.Application.Common.Interfaces;

namespace Seadora.Customer.Application.Consumers;

public class CustomerRegisteredConsumer : IConsumer<CustomerRegistered>
{
    private readonly ICustomerDbContext _context;

    public CustomerRegisteredConsumer(ICustomerDbContext context)
    {
        _context = context;
    }

    public async Task Consume(ConsumeContext<CustomerRegistered> context)
    {
        var msg = context.Message;
        var customerId = Guid.Parse(msg.UserId);
        var branchId = string.IsNullOrEmpty(msg.BranchId) ? Guid.Empty : Guid.Parse(msg.BranchId);

        var existing = await _context.Customers.FirstOrDefaultAsync(c => c.Id == customerId);
        if (existing == null)
        {
            var customer = new Seadora.Customer.Domain.Entities.Customer 
            {
                Id = customerId,
                BranchId = branchId,
                FullName = $"{msg.FirstName} {msg.LastName}",
                Email = Seadora.Customer.Domain.Entities.Customer.NormalizeEmail(msg.Email),
                CreatedUtc = DateTime.UtcNow,
                UpdatedUtc = DateTime.UtcNow
            };
            _context.Customers.Add(customer);
        }
        else
        {
            existing.FullName = $"{msg.FirstName} {msg.LastName}";
            existing.UpdatedUtc = DateTime.UtcNow;
            _context.Customers.Update(existing);
        }
        await _context.SaveChangesAsync(context.CancellationToken);
    }
}
