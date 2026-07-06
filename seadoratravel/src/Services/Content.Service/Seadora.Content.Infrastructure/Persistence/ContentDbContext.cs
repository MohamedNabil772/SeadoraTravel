using Microsoft.EntityFrameworkCore;
using Seadora.Content.Application.Common.Interfaces;
using Seadora.Content.Domain.Entities;

namespace Seadora.Content.Infrastructure.Persistence;

public class ContentDbContext : DbContext, IContentDbContext
{
    public ContentDbContext(DbContextOptions<ContentDbContext> options) : base(options) { }

    public DbSet<Destination> Destinations => Set<Destination>();
    public DbSet<Tour> Tours => Set<Tour>();
    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure JSON conversion for dictionaries
        modelBuilder.Entity<Category>().Property(c => c.Names).HasColumnType("jsonb");
        
        modelBuilder.Entity<Destination>().Property(d => d.Names).HasColumnType("jsonb");
        modelBuilder.Entity<Destination>().Property(d => d.Descriptions).HasColumnType("jsonb");
        
        modelBuilder.Entity<Tour>().Property(t => t.Names).HasColumnType("jsonb");
        modelBuilder.Entity<Tour>().Property(t => t.Descriptions).HasColumnType("jsonb");
    }
}
