using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Seadora.Content.Infrastructure.Persistence;

public class ContentDbContextFactory : IDesignTimeDbContextFactory<ContentDbContext>
{
    public ContentDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ContentDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Database=SeadoraContentDb;Username=postgres;Password=postgres");

        return new ContentDbContext(optionsBuilder.Options);
    }
}
