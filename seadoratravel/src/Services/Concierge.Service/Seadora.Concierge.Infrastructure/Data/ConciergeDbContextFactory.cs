using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Seadora.Concierge.Infrastructure.Data;

public class ConciergeDbContextFactory : IDesignTimeDbContextFactory<ConciergeDbContext>
{
    public ConciergeDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ConciergeDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Database=Seadora_Concierge;Username=postgres;Password=postgres");
        return new ConciergeDbContext(optionsBuilder.Options);
    }
}
