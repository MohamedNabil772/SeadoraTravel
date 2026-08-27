using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Seadora.Customer.Infrastructure.Persistence;

// ponytail: design-time only — the connection string is never opened, it just builds the model.
public class CustomerDbContextFactory : IDesignTimeDbContextFactory<CustomerDbContext>
{
    public CustomerDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CustomerDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Database=Seadora_Customer;Username=postgres;Password=postgres");
        return new CustomerDbContext(optionsBuilder.Options);
    }
}
