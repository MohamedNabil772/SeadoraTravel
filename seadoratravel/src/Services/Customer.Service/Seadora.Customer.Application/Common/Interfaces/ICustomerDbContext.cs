using Microsoft.EntityFrameworkCore;
using Seadora.Customer.Domain.Entities;

namespace Seadora.Customer.Application.Common.Interfaces;

public interface ICustomerDbContext
{
    DbSet<Seadora.Customer.Domain.Entities.Customer> Customers { get; }
    DbSet<CustomerDocument> CustomerDocuments { get; }
    DbSet<CustomerBookingHistory> BookingHistory { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
