using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Seadora.Finance.Infrastructure.Persistence;

// ponytail: design-time only — the connection string is never opened, it just builds the model.
public class FinanceDbContextFactory : IDesignTimeDbContextFactory<FinanceDbContext>
{
    public FinanceDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<FinanceDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Database=Seadora_Finance;Username=postgres;Password=postgres");
        return new FinanceDbContext(optionsBuilder.Options);
    }
}
