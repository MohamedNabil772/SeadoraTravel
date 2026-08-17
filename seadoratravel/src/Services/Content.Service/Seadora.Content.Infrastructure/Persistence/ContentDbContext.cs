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
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<PaymentAgreement> PaymentAgreements => Set<PaymentAgreement>();
    public DbSet<Language> Languages => Set<Language>();
    public DbSet<Currency> Currencies => Set<Currency>();
    public DbSet<Nationality> Nationalities => Set<Nationality>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure JSON conversion for dictionaries/collections
        modelBuilder.Entity<Category>().Property(c => c.Names).HasColumnType("jsonb");
        modelBuilder.Entity<Category>().Property(c => c.Descriptions).HasColumnType("jsonb");
        
        modelBuilder.Entity<Destination>().Property(d => d.Names).HasColumnType("jsonb");
        modelBuilder.Entity<Destination>().Property(d => d.Descriptions).HasColumnType("jsonb");
        modelBuilder.Entity<Destination>().Property(d => d.Highlights).HasColumnType("jsonb");
        
        modelBuilder.Entity<Tour>().Property(t => t.Names).HasColumnType("jsonb");
        modelBuilder.Entity<Tour>().Property(t => t.Descriptions).HasColumnType("jsonb");
        modelBuilder.Entity<Tour>().Property(t => t.Highlights).HasColumnType("jsonb");
        modelBuilder.Entity<Tour>().Property(t => t.AvailablePickupTimes).HasColumnType("jsonb");

        // Owned types mapped to JSONB using ToJson()
        modelBuilder.Entity<Tour>().OwnsMany(t => t.Packages, b => b.ToJson());
        modelBuilder.Entity<Tour>().OwnsMany(t => t.Itinerary, b => b.ToJson());
        modelBuilder.Entity<Tour>().OwnsMany(t => t.Inclusions, b => b.ToJson());
        modelBuilder.Entity<Tour>().OwnsMany(t => t.Exclusions, b => b.ToJson());
        modelBuilder.Entity<Tour>().OwnsOne(t => t.ImportantInformation, b => b.ToJson());
        modelBuilder.Entity<Tour>().OwnsMany(t => t.Faqs, b => b.ToJson());
        modelBuilder.Entity<Tour>().OwnsMany(t => t.Addons, b => b.ToJson());
        modelBuilder.Entity<Tour>().OwnsMany(t => t.Media, b => b.ToJson());

        // Supplier relations
        modelBuilder.Entity<Supplier>()
            .HasOne(s => s.PaymentAgreement)
            .WithMany()
            .HasForeignKey(s => s.PaymentAgreementId);

        modelBuilder.Entity<Tour>()
            .HasOne(t => t.Supplier)
            .WithMany()
            .HasForeignKey(t => t.SupplierId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
