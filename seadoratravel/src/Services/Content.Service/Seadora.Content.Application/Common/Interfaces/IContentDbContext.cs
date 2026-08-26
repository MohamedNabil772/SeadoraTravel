using Microsoft.EntityFrameworkCore;
using Seadora.Content.Domain.Entities;

namespace Seadora.Content.Application.Common.Interfaces;

public interface IContentDbContext
{
    DbSet<Destination> Destinations { get; }
    DbSet<Tour> Tours { get; }
    DbSet<Category> Categories { get; }
    DbSet<Supplier> Suppliers { get; }
    DbSet<PaymentAgreement> PaymentAgreements { get; }
    DbSet<Language> Languages { get; }
    DbSet<TourType> TourTypes { get; }
    DbSet<Currency> Currencies { get; }
    DbSet<Nationality> Nationalities { get; }
    DbSet<Translation> Translations { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
