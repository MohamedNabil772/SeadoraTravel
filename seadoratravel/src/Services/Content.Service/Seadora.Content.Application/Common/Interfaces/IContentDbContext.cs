using Microsoft.EntityFrameworkCore;
using Seadora.Content.Domain.Entities;

namespace Seadora.Content.Application.Common.Interfaces;

public interface IContentDbContext
{
    DbSet<Destination> Destinations { get; }
    DbSet<Tour> Tours { get; }
    DbSet<Category> Categories { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
