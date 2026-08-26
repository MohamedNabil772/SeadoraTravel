using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Seadora.Booking.Infrastructure.Persistence;

// ponytail: design-time only — the connection string is never opened, it just builds the model.
public class BookingDbContextFactory : IDesignTimeDbContextFactory<BookingDbContext>
{
    public BookingDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BookingDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Database=Seadora_Booking;Username=postgres;Password=postgres");
        return new BookingDbContext(optionsBuilder.Options);
    }
}
