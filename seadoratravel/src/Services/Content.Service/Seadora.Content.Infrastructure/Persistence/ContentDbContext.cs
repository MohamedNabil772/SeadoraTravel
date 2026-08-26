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
    public DbSet<TourType> TourTypes => Set<TourType>();
    public DbSet<Currency> Currencies => Set<Currency>();
    public DbSet<Nationality> Nationalities => Set<Nationality>();
    public DbSet<Seadora.Content.Domain.Entities.Translation> Translations => Set<Seadora.Content.Domain.Entities.Translation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure JSON conversion for dictionaries/collections
        modelBuilder.Entity<Seadora.Content.Domain.Entities.Translation>().Property(t => t.Values).HasColumnType("jsonb");
        modelBuilder.Entity<Seadora.Content.Domain.Entities.Translation>().HasIndex(t => new { t.Key, t.Namespace }).IsUnique();

        modelBuilder.Entity<Language>().HasIndex(l => l.Code).IsUnique();
        modelBuilder.Entity<Language>().HasIndex(l => l.Order);

        modelBuilder.Entity<Currency>().HasIndex(c => c.Code).IsUnique();
        modelBuilder.Entity<Nationality>().HasIndex(n => n.Code).IsUnique();
        modelBuilder.Entity<Nationality>().HasIndex(n => n.CountryName);

        modelBuilder.Entity<Category>().Property(c => c.Names).HasColumnType("jsonb");
        modelBuilder.Entity<Category>().Property(c => c.Descriptions).HasColumnType("jsonb");
        
        modelBuilder.Entity<Destination>().Property(d => d.Names).HasColumnType("jsonb");
        modelBuilder.Entity<Destination>().Property(d => d.Descriptions).HasColumnType("jsonb");
        modelBuilder.Entity<Destination>().Property(d => d.Highlights).HasColumnType("jsonb");
        
        modelBuilder.Entity<Tour>().Property(t => t.Names).HasColumnType("jsonb");
        modelBuilder.Entity<Tour>().Property(t => t.Descriptions).HasColumnType("jsonb");
        modelBuilder.Entity<Tour>().Property(t => t.Highlights).HasColumnType("jsonb");
        modelBuilder.Entity<Tour>().Property(t => t.AvailablePickupTimes).HasColumnType("jsonb");


        var jsonOptions = (System.Text.Json.JsonSerializerOptions?)null;

        modelBuilder.Entity<Tour>().Property(t => t.Packages)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, jsonOptions),
                v => System.Text.Json.JsonSerializer.Deserialize<List<TourPackage>>(v, jsonOptions) ?? new List<TourPackage>())
            .HasColumnType("jsonb");

        modelBuilder.Entity<Tour>().Property(t => t.Itinerary)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, jsonOptions),
                v => System.Text.Json.JsonSerializer.Deserialize<List<TourItinerary>>(v, jsonOptions) ?? new List<TourItinerary>())
            .HasColumnType("jsonb");

        modelBuilder.Entity<Tour>().Property(t => t.Inclusions)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, jsonOptions),
                v => System.Text.Json.JsonSerializer.Deserialize<List<TourInclusion>>(v, jsonOptions) ?? new List<TourInclusion>())
            .HasColumnType("jsonb");

        modelBuilder.Entity<Tour>().Property(t => t.Exclusions)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, jsonOptions),
                v => System.Text.Json.JsonSerializer.Deserialize<List<TourInclusion>>(v, jsonOptions) ?? new List<TourInclusion>())
            .HasColumnType("jsonb");

        modelBuilder.Entity<Tour>().Property(t => t.ImportantInformation)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, jsonOptions),
                v => System.Text.Json.JsonSerializer.Deserialize<ImportantInfo>(v, jsonOptions) ?? new ImportantInfo())
            .HasColumnType("jsonb");

        modelBuilder.Entity<Tour>().Property(t => t.Faqs)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, jsonOptions),
                v => System.Text.Json.JsonSerializer.Deserialize<List<TourFaq>>(v, jsonOptions) ?? new List<TourFaq>())
            .HasColumnType("jsonb");

        modelBuilder.Entity<Tour>().Property(t => t.Addons)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, jsonOptions),
                v => System.Text.Json.JsonSerializer.Deserialize<List<TourAddon>>(v, jsonOptions) ?? new List<TourAddon>())
            .HasColumnType("jsonb");

        modelBuilder.Entity<Tour>().Property(t => t.Media)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, jsonOptions),
                v => System.Text.Json.JsonSerializer.Deserialize<List<TourMedia>>(v, jsonOptions) ?? new List<TourMedia>())
            .HasColumnType("jsonb");

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

        // TourType configurations
        modelBuilder.Entity<TourType>().Property(tt => tt.Names).HasColumnType("jsonb");
        modelBuilder.Entity<TourType>().Property(tt => tt.Descriptions).HasColumnType("jsonb");
        modelBuilder.Entity<TourType>().HasIndex(tt => tt.Code).IsUnique();

        modelBuilder.Entity<Tour>()
            .HasOne(t => t.TourType)
            .WithMany(tt => tt.Tours)
            .HasForeignKey(t => t.TourTypeId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Tour>()
            .HasOne(t => t.Category)
            .WithMany(c => c.Tours)
            .HasForeignKey(t => t.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Tour>()
            .HasOne(t => t.Destination)
            .WithMany(d => d.Tours)
            .HasForeignKey(t => t.DestinationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
