using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Seadora.Support.Infrastructure.Data;

public class SupportDbContextFactory : IDesignTimeDbContextFactory<SupportDbContext>
{
    public SupportDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<SupportDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Database=Seadora_Support;Username=postgres;Password=postgres");
        return new SupportDbContext(optionsBuilder.Options);
    }
}
