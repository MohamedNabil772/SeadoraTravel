using Microsoft.EntityFrameworkCore;
using Seadora.Concierge.Application.Commands;
using Seadora.Concierge.Domain.Entities;

namespace Seadora.Concierge.Infrastructure.Data;

public class ConciergeDbContext : DbContext, IConciergeDbContext
{
    public ConciergeDbContext(DbContextOptions<ConciergeDbContext> options) : base(options)
    {
    }

    public DbSet<ConversationSession> ConversationSessions { get; set; } = null!;
    public DbSet<ConversationMessage> ConversationMessages { get; set; } = null!;
    public DbSet<TourCatalogIndex> TourCatalogIndices { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ConversationSession>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasMany(e => e.Messages).WithOne(e => e.Session).HasForeignKey(e => e.SessionId);
        });

        modelBuilder.Entity<ConversationMessage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Role).HasMaxLength(50);
        });

        modelBuilder.Entity<TourCatalogIndex>(entity =>
        {
            entity.HasKey(e => e.TourId);
            entity.Property(e => e.Names).HasColumnType("jsonb");
            entity.Property(e => e.Descriptions).HasColumnType("jsonb");
            entity.Property(e => e.PriceEur).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Rating).HasColumnType("decimal(3,2)");
        });
    }
}
