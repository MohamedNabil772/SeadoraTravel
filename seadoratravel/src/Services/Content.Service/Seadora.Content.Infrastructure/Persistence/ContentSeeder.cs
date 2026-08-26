using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Seadora.Content.Domain.Entities;
using Seadora.Contracts.Enums;
using System.Linq;

namespace Seadora.Content.Infrastructure.Persistence;

public static class ContentSeeder
{
    public static async Task InitializeAsync(ContentDbContext context)
    {
        try
        {
            await context.Database.MigrateAsync();
        }
        catch { }

        await context.Database.ExecuteSqlAsync($@"
            CREATE TABLE IF NOT EXISTS ""Languages"" (
                ""Id"" uuid NOT NULL PRIMARY KEY,
                ""Code"" text NOT NULL,
                ""Name"" text NOT NULL,
                ""NativeName"" text NOT NULL DEFAULT '',
                ""FlagEmoji"" text NOT NULL DEFAULT '',
                ""IsRtl"" boolean NOT NULL DEFAULT false,
                ""IsDefault"" boolean NOT NULL DEFAULT false,
                ""Order"" integer NOT NULL DEFAULT 0,
                ""IsActive"" boolean NOT NULL DEFAULT true
            );
            ALTER TABLE ""Languages"" ADD COLUMN IF NOT EXISTS ""FlagEmoji"" text DEFAULT '';
            ALTER TABLE ""Languages"" ADD COLUMN IF NOT EXISTS ""IsRtl"" boolean DEFAULT false;
            ALTER TABLE ""Languages"" ADD COLUMN IF NOT EXISTS ""IsDefault"" boolean DEFAULT false;
            ALTER TABLE ""Languages"" ADD COLUMN IF NOT EXISTS ""Order"" integer DEFAULT 0;
            CREATE TABLE IF NOT EXISTS ""Currencies"" (
                ""Id"" uuid NOT NULL PRIMARY KEY,
                ""Code"" text NOT NULL,
                ""Name"" text NOT NULL,
                ""Symbol"" text NOT NULL,
                ""ExchangeRate"" numeric NOT NULL DEFAULT 1.0,
                ""LiveExchangeRate"" numeric NULL,
                ""IsBase"" boolean NOT NULL DEFAULT false,
                ""IsManualRate"" boolean NOT NULL DEFAULT false,
                ""LastRateSyncAt"" timestamp with time zone NULL,
                ""IsActive"" boolean NOT NULL DEFAULT true
            );
            ALTER TABLE ""Currencies"" ADD COLUMN IF NOT EXISTS ""LiveExchangeRate"" numeric NULL;
            ALTER TABLE ""Currencies"" ADD COLUMN IF NOT EXISTS ""IsBase"" boolean NOT NULL DEFAULT false;
            ALTER TABLE ""Currencies"" ADD COLUMN IF NOT EXISTS ""IsManualRate"" boolean NOT NULL DEFAULT false;
            ALTER TABLE ""Currencies"" ADD COLUMN IF NOT EXISTS ""LastRateSyncAt"" timestamp with time zone NULL;

            CREATE TABLE IF NOT EXISTS ""Nationalities"" (
                ""Id"" uuid NOT NULL PRIMARY KEY,
                ""Code"" text NOT NULL,
                ""CountryName"" text NOT NULL DEFAULT '',
                ""NationalityName"" text NOT NULL DEFAULT '',
                ""FlagEmoji"" text NOT NULL DEFAULT '',
                ""IsActive"" boolean NOT NULL DEFAULT true
            );
            ALTER TABLE ""Nationalities"" ADD COLUMN IF NOT EXISTS ""Name"" text DEFAULT '';
            ALTER TABLE ""Nationalities"" ADD COLUMN IF NOT EXISTS ""CountryName"" text NOT NULL DEFAULT '';
            ALTER TABLE ""Nationalities"" ADD COLUMN IF NOT EXISTS ""NationalityName"" text NOT NULL DEFAULT '';
            ALTER TABLE ""Nationalities"" ADD COLUMN IF NOT EXISTS ""FlagEmoji"" text NOT NULL DEFAULT '';
            CREATE TABLE IF NOT EXISTS ""Translations"" (
                ""Id"" uuid NOT NULL PRIMARY KEY,
                ""Key"" text NOT NULL,
                ""Namespace"" text NOT NULL DEFAULT 'common',
                ""Values"" jsonb NOT NULL DEFAULT '{{}}',
                ""UpdatedAt"" timestamp with time zone NOT NULL DEFAULT NOW()
            );
            CREATE UNIQUE INDEX IF NOT EXISTS ""IX_Translations_Key_Namespace"" ON ""Translations"" (""Key"", ""Namespace"");
            ALTER TABLE ""Categories"" DROP COLUMN IF EXISTS ""Icon"";
            ALTER TABLE ""Categories"" ADD COLUMN IF NOT EXISTS ""CoverImageUrl"" text DEFAULT '';
            ALTER TABLE ""Categories"" ADD COLUMN IF NOT EXISTS ""IconName"" text DEFAULT '';
            ALTER TABLE ""Categories"" ADD COLUMN IF NOT EXISTS ""CustomIconUrl"" text DEFAULT '';
            ALTER TABLE ""Categories"" ADD COLUMN IF NOT EXISTS ""Order"" integer DEFAULT 0;
            ALTER TABLE ""Categories"" ADD COLUMN IF NOT EXISTS ""Names"" jsonb DEFAULT '{{}}';
            ALTER TABLE ""Categories"" ADD COLUMN IF NOT EXISTS ""Descriptions"" jsonb DEFAULT '{{}}';
            
            ALTER TABLE ""Destinations"" DROP COLUMN IF EXISTS ""Flag"";
            ALTER TABLE ""Destinations"" DROP COLUMN IF EXISTS ""Latitude"";
            ALTER TABLE ""Destinations"" DROP COLUMN IF EXISTS ""Longitude"";
            ALTER TABLE ""Destinations"" ADD COLUMN IF NOT EXISTS ""FlagEmoji"" text DEFAULT '';
            ALTER TABLE ""Destinations"" ADD COLUMN IF NOT EXISTS ""Highlights"" jsonb DEFAULT '{{}}';
            ALTER TABLE ""Destinations"" ADD COLUMN IF NOT EXISTS ""Names"" jsonb DEFAULT '{{}}';
            ALTER TABLE ""Destinations"" ADD COLUMN IF NOT EXISTS ""Descriptions"" jsonb DEFAULT '{{}}';

            ALTER TABLE ""Tours"" ADD COLUMN IF NOT EXISTS ""Names"" jsonb DEFAULT '{{}}';
            ALTER TABLE ""Tours"" ADD COLUMN IF NOT EXISTS ""Descriptions"" jsonb DEFAULT '{{}}';
            ALTER TABLE ""Tours"" ADD COLUMN IF NOT EXISTS ""Highlights"" jsonb DEFAULT '{{}}';
            ALTER TABLE ""Tours"" ADD COLUMN IF NOT EXISTS ""AvailablePickupTimes"" jsonb DEFAULT '[]';
            ALTER TABLE ""Tours"" ADD COLUMN IF NOT EXISTS ""Packages"" jsonb DEFAULT '[]';
            ALTER TABLE ""Tours"" ADD COLUMN IF NOT EXISTS ""Itinerary"" jsonb DEFAULT '[]';
            ALTER TABLE ""Tours"" ADD COLUMN IF NOT EXISTS ""Inclusions"" jsonb DEFAULT '[]';
            ALTER TABLE ""Tours"" ADD COLUMN IF NOT EXISTS ""Exclusions"" jsonb DEFAULT '[]';
            ALTER TABLE ""Tours"" ADD COLUMN IF NOT EXISTS ""ImportantInformation"" jsonb DEFAULT '{{}}';
            ALTER TABLE ""Tours"" ADD COLUMN IF NOT EXISTS ""Faqs"" jsonb DEFAULT '[]';
            ALTER TABLE ""Tours"" ADD COLUMN IF NOT EXISTS ""Addons"" jsonb DEFAULT '[]';
            ALTER TABLE ""Tours"" ADD COLUMN IF NOT EXISTS ""Media"" jsonb DEFAULT '[]';
            ALTER TABLE ""Tours"" ADD COLUMN IF NOT EXISTS ""PickupTimeType"" text DEFAULT 'FixedSlots';
            ALTER TABLE ""Tours"" ADD COLUMN IF NOT EXISTS ""OriginalPrice"" numeric NULL;
            ALTER TABLE ""Tours"" ADD COLUMN IF NOT EXISTS ""DiscountPercentage"" numeric NULL;
            ALTER TABLE ""Tours"" ADD COLUMN IF NOT EXISTS ""TourTypeId"" uuid NULL;
            ALTER TABLE ""Tours"" ADD COLUMN IF NOT EXISTS ""GroupMinCapacity"" integer NULL DEFAULT 1;
            ALTER TABLE ""Tours"" ADD COLUMN IF NOT EXISTS ""GroupMaxCapacity"" integer NULL DEFAULT 20;

            CREATE TABLE IF NOT EXISTS ""TourTypes"" (
                ""Id"" uuid NOT NULL PRIMARY KEY,
                ""Code"" text NOT NULL,
                ""Names"" jsonb NOT NULL DEFAULT '{{}}',
                ""Descriptions"" jsonb NOT NULL DEFAULT '{{}}',
                ""Icon"" text NOT NULL DEFAULT '⛵',
                ""Order"" integer NOT NULL DEFAULT 0,
                ""IsActive"" boolean NOT NULL DEFAULT true
            );
            ALTER TABLE ""TourTypes"" ADD COLUMN IF NOT EXISTS ""AllocationModel"" integer NOT NULL DEFAULT 0;
            ALTER TABLE ""TourTypes"" ADD COLUMN IF NOT EXISTS ""DefaultMinCapacity"" integer NULL;
            ALTER TABLE ""TourTypes"" ADD COLUMN IF NOT EXISTS ""DefaultMaxCapacity"" integer NULL;
            ALTER TABLE ""TourTypes"" ADD COLUMN IF NOT EXISTS ""RequiresGuestDetails"" boolean NOT NULL DEFAULT false;
            ALTER TABLE ""TourTypes"" ADD COLUMN IF NOT EXISTS ""RequiresPassport"" boolean NOT NULL DEFAULT false;
            ALTER TABLE ""TourTypes"" ADD COLUMN IF NOT EXISTS ""PayLaterAllowed"" boolean NOT NULL DEFAULT true;
            CREATE UNIQUE INDEX IF NOT EXISTS ""IX_TourTypes_Code"" ON ""TourTypes"" (""Code"");
        ");

        // Seed Tour Types
        if (!await context.TourTypes.AnyAsync())
        {
            var defaultTourTypes = new List<TourType>
            {
                new TourType
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Code = "GROUP",
                    Icon = "⛵",
                    Order = 1,
                    IsActive = true,
                    Names = new Dictionary<string, string> { { "en", "Group Tour" }, { "de", "Gruppentour" }, { "it", "Tour di Gruppo" }, { "fr", "Visite en Groupe" }, { "ru", "Групповой Тур" } },
                    Descriptions = new Dictionary<string, string> { { "en", "Shared guided excursion with fellow travelers." } },
                    AllocationModel = AllocationModel.Shared,
                    DefaultMinCapacity = 1,
                    DefaultMaxCapacity = 30,
                    RequiresGuestDetails = true,
                    RequiresPassport = false,
                    PayLaterAllowed = true
                },
                new TourType
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Code = "PRIVATE",
                    Icon = "👑",
                    Order = 2,
                    IsActive = true,
                    Names = new Dictionary<string, string> { { "en", "Private Tour" }, { "de", "Privattour" }, { "it", "Tour Privato" }, { "fr", "Visite Privée" }, { "ru", "Индивидуальный Тур" } },
                    Descriptions = new Dictionary<string, string> { { "en", "Exclusive tour with dedicated guide and private transportation." } },
                    AllocationModel = AllocationModel.WholeUnit,
                    DefaultMinCapacity = 1,
                    DefaultMaxCapacity = 12,
                    RequiresGuestDetails = true,
                    RequiresPassport = false,
                    PayLaterAllowed = true
                },
                new TourType
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    Code = "VIP",
                    Icon = "✨",
                    Order = 3,
                    IsActive = true,
                    Names = new Dictionary<string, string> { { "en", "VIP Luxury Excursion" }, { "de", "VIP Luxus Exkursion" }, { "it", "Escursione VIP di Lusso" }, { "fr", "Excursion VIP de Luxe" }, { "ru", "VIP Люкс Экскурсия" } },
                    Descriptions = new Dictionary<string, string> { { "en", "Premium bespoke luxury experience with white-glove concierge services." } },
                    AllocationModel = AllocationModel.WholeUnit,
                    DefaultMinCapacity = 1,
                    DefaultMaxCapacity = 8,
                    RequiresGuestDetails = true,
                    RequiresPassport = false,
                    PayLaterAllowed = false
                },
                new TourType
                {
                    Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                    Code = "YACHT",
                    Icon = "🛥️",
                    Order = 4,
                    IsActive = true,
                    Names = new Dictionary<string, string> { { "en", "Yacht & Boat Charter" }, { "de", "Yacht- & Bootscharter" }, { "it", "Noleggio Yacht e Barche" }, { "fr", "Location de Yacht et Bateau" }, { "ru", "Аренда Яхт и Катеров" } },
                    Descriptions = new Dictionary<string, string> { { "en", "Private or premium sea cruising, island hopping, and marine excursions." } },
                    AllocationModel = AllocationModel.WholeUnit,
                    DefaultMinCapacity = 1,
                    DefaultMaxCapacity = 12,
                    RequiresGuestDetails = true,
                    RequiresPassport = false,
                    PayLaterAllowed = false
                },
                new TourType
                {
                    Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                    Code = "SHORE_EXCURSION",
                    Icon = "⚓",
                    Order = 5,
                    IsActive = true,
                    Names = new Dictionary<string, string> { { "en", "Shore Excursion" }, { "de", "Landausflug" }, { "it", "Escursione a Terra" }, { "fr", "Excursion à Terre" }, { "ru", "Береговая Экскурсия" } },
                    Descriptions = new Dictionary<string, string> { { "en", "Tailored cruise ship port excursions with guaranteed on-time return." } },
                    // ponytail: passport required - cruise pier pickups need document checks.
                    AllocationModel = AllocationModel.Shared,
                    DefaultMinCapacity = 1,
                    DefaultMaxCapacity = 30,
                    RequiresGuestDetails = true,
                    RequiresPassport = true,
                    PayLaterAllowed = true
                },
                new TourType
                {
                    Id = Guid.Parse("66666666-6666-6666-6666-666666666666"),
                    Code = "MULTI_DAY",
                    Icon = "🏔️",
                    Order = 6,
                    IsActive = true,
                    Names = new Dictionary<string, string> { { "en", "Multi-Day Expedition" }, { "de", "Mehrtägige Expedition" }, { "it", "Spedizione di Più Giorni" }, { "fr", "Expédition Multi-Jours" }, { "ru", "Многодневная Экспедиция" } },
                    Descriptions = new Dictionary<string, string> { { "en", "Immersive comprehensive travel journeys spanning multiple days." } },
                    // ponytail: WholeUnit - multi-day itineraries are sold as a whole departure; passport needed for hotel check-in.
                    AllocationModel = AllocationModel.WholeUnit,
                    DefaultMinCapacity = 2,
                    DefaultMaxCapacity = 16,
                    RequiresGuestDetails = true,
                    RequiresPassport = true,
                    PayLaterAllowed = false
                }
            };
            context.TourTypes.AddRange(defaultTourTypes);
            await context.SaveChangesAsync();
        }

        // Ensure all seeded tours have TourTypeId populated so counts and badges reflect in the grids
        var unlinkedTours = await context.Tours.Where(t => t.TourTypeId == null).ToListAsync();
        if (unlinkedTours.Any())
        {
            var groupTypeId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var privateTypeId = Guid.Parse("22222222-2222-2222-2222-222222222222");
            var vipTypeId = Guid.Parse("33333333-3333-3333-3333-333333333333");
            var yachtTypeId = Guid.Parse("44444444-4444-4444-4444-444444444444");
            var shoreTypeId = Guid.Parse("55555555-5555-5555-5555-555555555555");
            var multiDayTypeId = Guid.Parse("66666666-6666-6666-6666-666666666666");

            foreach (var tour in unlinkedTours)
            {
                var title = tour.Names?.GetValueOrDefault("en")?.ToLowerInvariant() ?? "";
                if (title.Contains("vip") || title.Contains("luxury"))
                    tour.TourTypeId = vipTypeId;
                else if (title.Contains("yacht") || title.Contains("boat") || title.Contains("submarine") || title.Contains("parasailing"))
                    tour.TourTypeId = yachtTypeId;
                else if (title.Contains("private") || title.Contains("balloon"))
                    tour.TourTypeId = privateTypeId;
                else if (title.Contains("simbel") || title.Contains("expedition"))
                    tour.TourTypeId = multiDayTypeId;
                else
                    tour.TourTypeId = groupTypeId;
            }
            await context.SaveChangesAsync();
        }

        if (await context.Categories.AnyAsync()) return;

        // Seed Languages
        if (!await context.Languages.AnyAsync())
        {
            context.Languages.AddRange(new List<Language>
            {
                new Language { Id = Guid.NewGuid(), Code = "en", Name = "English", NativeName = "English", FlagEmoji = "🇬🇧", IsRtl = false, IsDefault = true, Order = 1, IsActive = true },
                new Language { Id = Guid.NewGuid(), Code = "de", Name = "German", NativeName = "Deutsch", FlagEmoji = "🇩🇪", IsRtl = false, IsDefault = false, Order = 2, IsActive = true },
                new Language { Id = Guid.NewGuid(), Code = "it", Name = "Italian", NativeName = "Italiano", FlagEmoji = "🇮🇹", IsRtl = false, IsDefault = false, Order = 3, IsActive = true },
                new Language { Id = Guid.NewGuid(), Code = "fr", Name = "French", NativeName = "Français", FlagEmoji = "🇫🇷", IsRtl = false, IsDefault = false, Order = 4, IsActive = true },
                new Language { Id = Guid.NewGuid(), Code = "ru", Name = "Russian", NativeName = "Русский", FlagEmoji = "🇷🇺", IsRtl = false, IsDefault = false, Order = 5, IsActive = true }
            });
            await context.SaveChangesAsync();
        }

        // Seed Translations
        if (!await context.Translations.AnyAsync())
        {
            context.Translations.AddRange(new List<Translation>
            {
                new Translation
                {
                    Id = Guid.NewGuid(),
                    Key = "Home",
                    Namespace = "nav",
                    Values = new Dictionary<string, string> { { "en", "Home" }, { "de", "Startseite" }, { "fr", "Accueil" }, { "it", "Home" }, { "ru", "Главная" } },
                    UpdatedAt = DateTime.UtcNow
                },
                new Translation
                {
                    Id = Guid.NewGuid(),
                    Key = "Tours",
                    Namespace = "nav",
                    Values = new Dictionary<string, string> { { "en", "Tours" }, { "de", "Touren" }, { "fr", "Circuits" }, { "it", "Tour" }, { "ru", "Туры" } },
                    UpdatedAt = DateTime.UtcNow
                },
                new Translation
                {
                    Id = Guid.NewGuid(),
                    Key = "BookNow",
                    Namespace = "common",
                    Values = new Dictionary<string, string> { { "en", "Book Now" }, { "de", "Jetzt Buchen" }, { "fr", "Réserver" }, { "it", "Prenota Ora" }, { "ru", "Забронировать" } },
                    UpdatedAt = DateTime.UtcNow
                }
            });
            await context.SaveChangesAsync();
        }

                // Seed Currencies (EUR Base, USD, EGP)
        if (!await context.Currencies.AnyAsync())
        {
            context.Currencies.AddRange(new List<Currency>
            {
                new Currency { Id = Guid.NewGuid(), Code = "EUR", Name = "Euro", Symbol = "?", ExchangeRate = 1.0m, LiveExchangeRate = 1.0m, IsBase = true, IsManualRate = false, LastRateSyncAt = DateTime.UtcNow, IsActive = true },
                new Currency { Id = Guid.NewGuid(), Code = "USD", Name = "US Dollar", Symbol = "$", ExchangeRate = 1.085m, LiveExchangeRate = 1.085m, IsBase = false, IsManualRate = false, LastRateSyncAt = DateTime.UtcNow, IsActive = true },
                new Currency { Id = Guid.NewGuid(), Code = "EGP", Name = "Egyptian Pound", Symbol = "E?", ExchangeRate = 52.50m, LiveExchangeRate = 52.50m, IsBase = false, IsManualRate = false, LastRateSyncAt = DateTime.UtcNow, IsActive = true }
            });
            await context.SaveChangesAsync();
        }

                // Seed All World Nationalities (195 Sovereign Nations)
        if (!await context.Nationalities.AnyAsync())
        {
            context.Nationalities.AddRange(new List<Nationality>
        {
            new Nationality { Id = Guid.NewGuid(), Code = "AF", CountryName = "Afghanistan", NationalityName = "Afghan", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "AL", CountryName = "Albania", NationalityName = "Albanian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "DZ", CountryName = "Algeria", NationalityName = "Algerian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "AD", CountryName = "Andorra", NationalityName = "Andorran", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "AO", CountryName = "Angola", NationalityName = "Angolan", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "AG", CountryName = "Antigua and Barbuda", NationalityName = "Antiguan", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "AR", CountryName = "Argentina", NationalityName = "Argentine", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "AM", CountryName = "Armenia", NationalityName = "Armenian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "AU", CountryName = "Australia", NationalityName = "Australian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "AT", CountryName = "Austria", NationalityName = "Austrian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "AZ", CountryName = "Azerbaijan", NationalityName = "Azerbaijani", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "BS", CountryName = "Bahamas", NationalityName = "Bahamian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "BH", CountryName = "Bahrain", NationalityName = "Bahraini", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "BD", CountryName = "Bangladesh", NationalityName = "Bangladeshi", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "BB", CountryName = "Barbados", NationalityName = "Barbadian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "BY", CountryName = "Belarus", NationalityName = "Belarusian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "BE", CountryName = "Belgium", NationalityName = "Belgian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "BZ", CountryName = "Belize", NationalityName = "Belizean", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "BJ", CountryName = "Benin", NationalityName = "Beninese", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "BT", CountryName = "Bhutan", NationalityName = "Bhutanese", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "BO", CountryName = "Bolivia", NationalityName = "Bolivian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "BA", CountryName = "Bosnia and Herzegovina", NationalityName = "Bosnian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "BW", CountryName = "Botswana", NationalityName = "Motswana", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "BR", CountryName = "Brazil", NationalityName = "Brazilian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "BN", CountryName = "Brunei", NationalityName = "Bruneian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "BG", CountryName = "Bulgaria", NationalityName = "Bulgarian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "BF", CountryName = "Burkina Faso", NationalityName = "Burkinabe", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "BI", CountryName = "Burundi", NationalityName = "Burundian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "CV", CountryName = "Cabo Verde", NationalityName = "Cabo Verdean", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "KH", CountryName = "Cambodia", NationalityName = "Cambodian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "CM", CountryName = "Cameroon", NationalityName = "Cameroonian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "CA", CountryName = "Canada", NationalityName = "Canadian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "CF", CountryName = "Central African Republic", NationalityName = "Central African", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "TD", CountryName = "Chad", NationalityName = "Chadian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "CL", CountryName = "Chile", NationalityName = "Chilean", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "CN", CountryName = "China", NationalityName = "Chinese", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "CO", CountryName = "Colombia", NationalityName = "Colombian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "KM", CountryName = "Comoros", NationalityName = "Comorian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "CG", CountryName = "Congo", NationalityName = "Congolese", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "CD", CountryName = "DR Congo", NationalityName = "Congolese", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "CR", CountryName = "Costa Rica", NationalityName = "Costa Rican", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "CI", CountryName = "Ivory Coast", NationalityName = "Ivorian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "HR", CountryName = "Croatia", NationalityName = "Croatian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "CU", CountryName = "Cuba", NationalityName = "Cuban", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "CY", CountryName = "Cyprus", NationalityName = "Cypriot", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "CZ", CountryName = "Czech Republic", NationalityName = "Czech", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "DK", CountryName = "Denmark", NationalityName = "Danish", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "DJ", CountryName = "Djibouti", NationalityName = "Djiboutian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "DM", CountryName = "Dominica", NationalityName = "Dominican", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "DO", CountryName = "Dominican Republic", NationalityName = "Dominican", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "EC", CountryName = "Ecuador", NationalityName = "Ecuadorian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "EG", CountryName = "Egypt", NationalityName = "Egyptian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "SV", CountryName = "El Salvador", NationalityName = "Salvadoran", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "GQ", CountryName = "Equatorial Guinea", NationalityName = "Equatorial Guinean", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "ER", CountryName = "Eritrea", NationalityName = "Eritrean", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "EE", CountryName = "Estonia", NationalityName = "Estonian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "SZ", CountryName = "Eswatini", NationalityName = "Eswatini", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "ET", CountryName = "Ethiopia", NationalityName = "Ethiopian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "FJ", CountryName = "Fiji", NationalityName = "Fijian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "FI", CountryName = "Finland", NationalityName = "Finnish", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "FR", CountryName = "France", NationalityName = "French", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "GA", CountryName = "Gabon", NationalityName = "Gabonese", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "GM", CountryName = "Gambia", NationalityName = "Gambian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "GE", CountryName = "Georgia", NationalityName = "Georgian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "DE", CountryName = "Germany", NationalityName = "German", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "GH", CountryName = "Ghana", NationalityName = "Ghanaian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "GR", CountryName = "Greece", NationalityName = "Greek", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "GD", CountryName = "Grenada", NationalityName = "Grenadian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "GT", CountryName = "Guatemala", NationalityName = "Guatemalan", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "GN", CountryName = "Guinea", NationalityName = "Guinean", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "GW", CountryName = "Guinea-Bissau", NationalityName = "Bissau-Guinean", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "GY", CountryName = "Guyana", NationalityName = "Guyanese", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "HT", CountryName = "Haiti", NationalityName = "Haitian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "HN", CountryName = "Honduras", NationalityName = "Honduran", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "HU", CountryName = "Hungary", NationalityName = "Hungarian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "IS", CountryName = "Iceland", NationalityName = "Icelandic", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "IN", CountryName = "India", NationalityName = "Indian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "ID", CountryName = "Indonesia", NationalityName = "Indonesian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "IR", CountryName = "Iran", NationalityName = "Iranian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "IQ", CountryName = "Iraq", NationalityName = "Iraqi", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "IE", CountryName = "Ireland", NationalityName = "Irish", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "IL", CountryName = "Israel", NationalityName = "Israeli", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "IT", CountryName = "Italy", NationalityName = "Italian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "JM", CountryName = "Jamaica", NationalityName = "Jamaican", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "JP", CountryName = "Japan", NationalityName = "Japanese", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "JO", CountryName = "Jordan", NationalityName = "Jordanian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "KZ", CountryName = "Kazakhstan", NationalityName = "Kazakhstani", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "KE", CountryName = "Kenya", NationalityName = "Kenyan", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "KI", CountryName = "Kiribati", NationalityName = "I-Kiribati", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "KP", CountryName = "North Korea", NationalityName = "North Korean", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "KR", CountryName = "South Korea", NationalityName = "South Korean", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "KW", CountryName = "Kuwait", NationalityName = "Kuwaiti", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "KG", CountryName = "Kyrgyzstan", NationalityName = "Kyrgyz", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "LA", CountryName = "Laos", NationalityName = "Laotian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "LV", CountryName = "Latvia", NationalityName = "Latvian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "LB", CountryName = "Lebanon", NationalityName = "Lebanese", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "LS", CountryName = "Lesotho", NationalityName = "Basotho", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "LR", CountryName = "Liberia", NationalityName = "Liberian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "LY", CountryName = "Libya", NationalityName = "Libyan", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "LI", CountryName = "Liechtenstein", NationalityName = "Liechtensteiner", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "LT", CountryName = "Lithuania", NationalityName = "Lithuanian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "LU", CountryName = "Luxembourg", NationalityName = "Luxembourger", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "MG", CountryName = "Madagascar", NationalityName = "Malagasy", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "MW", CountryName = "Malawi", NationalityName = "Malawian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "MY", CountryName = "Malaysia", NationalityName = "Malaysian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "MV", CountryName = "Maldives", NationalityName = "Maldivian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "ML", CountryName = "Mali", NationalityName = "Malian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "MT", CountryName = "Malta", NationalityName = "Maltese", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "MH", CountryName = "Marshall Islands", NationalityName = "Marshallese", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "MR", CountryName = "Mauritania", NationalityName = "Mauritanian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "MU", CountryName = "Mauritius", NationalityName = "Mauritian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "MX", CountryName = "Mexico", NationalityName = "Mexican", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "FM", CountryName = "Micronesia", NationalityName = "Micronesian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "MD", CountryName = "Moldova", NationalityName = "Moldovan", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "MC", CountryName = "Monaco", NationalityName = "Monegasque", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "MN", CountryName = "Mongolia", NationalityName = "Mongolian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "ME", CountryName = "Montenegro", NationalityName = "Montenegrin", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "MA", CountryName = "Morocco", NationalityName = "Moroccan", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "MZ", CountryName = "Mozambique", NationalityName = "Mozambican", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "MM", CountryName = "Myanmar", NationalityName = "Burmese", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "NA", CountryName = "Namibia", NationalityName = "Namibian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "NR", CountryName = "Nauru", NationalityName = "Nauruan", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "NP", CountryName = "Nepal", NationalityName = "Nepali", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "NL", CountryName = "Netherlands", NationalityName = "Dutch", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "NZ", CountryName = "New Zealand", NationalityName = "New Zealander", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "NI", CountryName = "Nicaragua", NationalityName = "Nicaraguan", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "NE", CountryName = "Niger", NationalityName = "Nigerien", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "NG", CountryName = "Nigeria", NationalityName = "Nigerian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "MK", CountryName = "North Macedonia", NationalityName = "Macedonian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "NO", CountryName = "Norway", NationalityName = "Norwegian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "OM", CountryName = "Oman", NationalityName = "Omani", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "PK", CountryName = "Pakistan", NationalityName = "Pakistani", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "PW", CountryName = "Palau", NationalityName = "Palauan", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "PS", CountryName = "Palestine", NationalityName = "Palestinian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "PA", CountryName = "Panama", NationalityName = "Panamanian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "PG", CountryName = "Papua New Guinea", NationalityName = "Papua New Guinean", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "PY", CountryName = "Paraguay", NationalityName = "Paraguayan", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "PE", CountryName = "Peru", NationalityName = "Peruvian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "PH", CountryName = "Philippines", NationalityName = "Filipino", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "PL", CountryName = "Poland", NationalityName = "Polish", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "PT", CountryName = "Portugal", NationalityName = "Portuguese", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "QA", CountryName = "Qatar", NationalityName = "Qatari", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "RO", CountryName = "Romania", NationalityName = "Romanian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "RU", CountryName = "Russia", NationalityName = "Russian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "RW", CountryName = "Rwanda", NationalityName = "Rwandan", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "KN", CountryName = "Saint Kitts and Nevis", NationalityName = "Kittitian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "LC", CountryName = "Saint Lucia", NationalityName = "Saint Lucian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "VC", CountryName = "Saint Vincent and the Grenadines", NationalityName = "Vincentian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "WS", CountryName = "Samoa", NationalityName = "Samoan", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "SM", CountryName = "San Marino", NationalityName = "Sammarinese", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "ST", CountryName = "Sao Tome and Principe", NationalityName = "Sao Tomean", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "SA", CountryName = "Saudi Arabia", NationalityName = "Saudi", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "SN", CountryName = "Senegal", NationalityName = "Senegalese", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "RS", CountryName = "Serbia", NationalityName = "Serbian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "SC", CountryName = "Seychelles", NationalityName = "Seychellois", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "SL", CountryName = "Sierra Leone", NationalityName = "Sierra Leonean", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "SG", CountryName = "Singapore", NationalityName = "Singaporean", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "SK", CountryName = "Slovakia", NationalityName = "Slovak", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "SI", CountryName = "Slovenia", NationalityName = "Slovenian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "SB", CountryName = "Solomon Islands", NationalityName = "Solomon Islander", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "SO", CountryName = "Somalia", NationalityName = "Somali", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "ZA", CountryName = "South Africa", NationalityName = "South African", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "SS", CountryName = "South Sudan", NationalityName = "South Sudanese", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "ES", CountryName = "Spain", NationalityName = "Spanish", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "LK", CountryName = "Sri Lanka", NationalityName = "Sri Lankan", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "SD", CountryName = "Sudan", NationalityName = "Sudanese", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "SR", CountryName = "Suriname", NationalityName = "Surinamese", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "SE", CountryName = "Sweden", NationalityName = "Swedish", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "CH", CountryName = "Switzerland", NationalityName = "Swiss", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "SY", CountryName = "Syria", NationalityName = "Syrian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "TJ", CountryName = "Tajikistan", NationalityName = "Tajik", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "TZ", CountryName = "Tanzania", NationalityName = "Tanzanian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "TH", CountryName = "Thailand", NationalityName = "Thai", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "TL", CountryName = "Timor-Leste", NationalityName = "Timorese", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "TG", CountryName = "Togo", NationalityName = "Togolese", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "TO", CountryName = "Tonga", NationalityName = "Tongan", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "TT", CountryName = "Trinidad and Tobago", NationalityName = "Trinidadian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "TN", CountryName = "Tunisia", NationalityName = "Tunisian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "TR", CountryName = "Turkey", NationalityName = "Turkish", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "TM", CountryName = "Turkmenistan", NationalityName = "Turkmen", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "TV", CountryName = "Tuvalu", NationalityName = "Tuvaluan", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "UG", CountryName = "Uganda", NationalityName = "Ugandan", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "UA", CountryName = "Ukraine", NationalityName = "Ukrainian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "AE", CountryName = "United Arab Emirates", NationalityName = "Emirati", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "GB", CountryName = "United Kingdom", NationalityName = "British", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "US", CountryName = "United States", NationalityName = "American", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "UY", CountryName = "Uruguay", NationalityName = "Uruguayan", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "UZ", CountryName = "Uzbekistan", NationalityName = "Uzbek", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "VU", CountryName = "Vanuatu", NationalityName = "Ni-Vanuatu", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "VA", CountryName = "Vatican City", NationalityName = "Vatican", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "VE", CountryName = "Venezuela", NationalityName = "Venezuelan", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "VN", CountryName = "Vietnam", NationalityName = "Vietnamese", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "YE", CountryName = "Yemen", NationalityName = "Yemeni", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "ZM", CountryName = "Zambia", NationalityName = "Zambian", FlagEmoji = "????", IsActive = true },
            new Nationality { Id = Guid.NewGuid(), Code = "ZW", CountryName = "Zimbabwe", NationalityName = "Zimbabwean", FlagEmoji = "????", IsActive = true },
        }
            );
            await context.SaveChangesAsync();
        }

        // 0. Seed Payment Agreements
        if (!await context.PaymentAgreements.AnyAsync())
        {
            var agreements = new List<PaymentAgreement>
            {
                new PaymentAgreement { Id = Guid.Parse("00000000-0000-0000-0000-000000000091"), Name = "Daily" },
                new PaymentAgreement { Id = Guid.Parse("00000000-0000-0000-0000-000000000092"), Name = "Weekly" },
                new PaymentAgreement { Id = Guid.Parse("00000000-0000-0000-0000-000000000093"), Name = "Monthly" },
                new PaymentAgreement { Id = Guid.Parse("00000000-0000-0000-0000-000000000094"), Name = "Yearly" }
            };
            context.PaymentAgreements.AddRange(agreements);
            await context.SaveChangesAsync();
        }

        // 0.5 Seed Suppliers
        if (!await context.Suppliers.AnyAsync())
        {
            var agreements = await context.PaymentAgreements.ToListAsync();
            var suppliers = new List<Supplier>
            {
                new Supplier {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000081"),
                    NameAr = "مغامرات البحر الأحمر",
                    NameEn = "Red Sea Adventures",
                    BankAccountInfo = "EG9300020001000000012345678 - CIB Egypt",
                    PaymentAgreementId = agreements[1].Id
                },
                new Supplier {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000082"),
                    NameAr = "جولات القاهرة التاريخية",
                    NameEn = "Cairo Historic Tours",
                    BankAccountInfo = "EG5400030002000000098765432 - QNB Alahli",
                    PaymentAgreementId = agreements[2].Id
                },
                new Supplier {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000083"),
                    NameAr = "سفاري بدو سيناء",
                    NameEn = "Sinai Bedouin Safari",
                    BankAccountInfo = "EG1200050003000000055554444 - Banque Misr",
                    PaymentAgreementId = agreements[0].Id
                }
            };
            context.Suppliers.AddRange(suppliers);
            await context.SaveChangesAsync();
        }

        // 1. Seed Categories
        var categories = new List<Category>
        {
            new Category { 
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"), 
                Names = new Dictionary<string, string> { 
                    { "en", "Diving & Water Sports" }, { "de", "Tauchen & Wassersport" }, { "fr", "Plongée & Sports Nautiques" }, { "it", "Immersioni e Sport Acquatici" }, { "ru", "Дайвинг и водные виды спорта" } 
                },
                Descriptions = new Dictionary<string, string> { { "en", "Explore the vibrant underwater life." }, { "de", "Erkunden Sie das pulsierende Unterwasserleben." }, { "fr", "Explorez la vie sous-marine vibrante." }, { "it", "Esplora la vibrante vita sottomarina." }, { "ru", "Исследуйте яркую подводную жизнь." } },
                IconName = "diving",
                CoverImageUrl = "/images/categories/diving.jpg"
            },
            new Category { 
                Id = Guid.Parse("00000000-0000-0000-0000-000000000002"), 
                Names = new Dictionary<string, string> { 
                    { "en", "Culture & History" }, { "de", "Kultur & Geschichte" }, { "fr", "Culture & Histoire" }, { "it", "Cultura e Storia" }, { "ru", "Культура и история" } 
                },
                Descriptions = new Dictionary<string, string> { { "en", "Journey into ancient civilizations." }, { "de", "Reise in alte Zivilisationen." }, { "fr", "Voyage dans les anciennes civilisations." }, { "it", "Viaggio nelle antiche civiltà." }, { "ru", "Путешествие в древние цивилизации." } },
                IconName = "culture",
                CoverImageUrl = "/images/categories/culture.jpg"
            },
            new Category { 
                Id = Guid.Parse("00000000-0000-0000-0000-000000000003"), 
                Names = new Dictionary<string, string> { 
                    { "en", "Safari & Adventure" }, { "de", "Safari & Abenteuer" }, { "fr", "Safari & Aventure" }, { "it", "Safari e Avventura" }, { "ru", "Сафари и приключения" } 
                },
                Descriptions = new Dictionary<string, string> { { "en", "Thrilling desert adventures." }, { "de", "Aufregende Wüstenabenteuer." }, { "fr", "Aventures palpitantes dans le désert." }, { "it", "Emozionanti avventure nel deserto." }, { "ru", "Захватывающие приключения в пустыне." } },
                IconName = "safari",
                CoverImageUrl = "/images/categories/safari.jpg"
            },
            new Category { 
                Id = Guid.Parse("00000000-0000-0000-0000-000000000004"), 
                Names = new Dictionary<string, string> { 
                    { "en", "Boat & Sea Trips" }, { "de", "Boot- & Seefahrten" }, { "fr", "Excursions en bateau" }, { "it", "Gite in Barca e Mare" }, { "ru", "Морские прогулки" } 
                },
                Descriptions = new Dictionary<string, string> { { "en", "Relaxing trips on the sea." }, { "de", "Entspannende Ausflüge auf dem Meer." }, { "fr", "Voyages relaxants sur la mer." }, { "it", "Viaggi rilassanti sul mare." }, { "ru", "Расслабляющие поездки по морю." } },
                IconName = "boat",
                CoverImageUrl = "/images/categories/boat.jpg"
            }
        };
        if (!await context.Categories.AnyAsync())
        {
            context.Categories.AddRange(categories);
            await context.SaveChangesAsync();
        }

        // 2. Seed Destinations
        var destinations = new List<Destination>
        {
            new Destination { Id = Guid.Parse("00000000-0000-0000-0000-000000000011"), Names = new Dictionary<string, string> { { "en", "Hurghada" }, { "de", "Hurghada" }, { "fr", "Hurghada" }, { "it", "Hurghada" }, { "ru", "Хургада" } }, Descriptions = new Dictionary<string, string> { { "en", "The Red Sea Riviera" }, { "de", "Die Riviera des Roten Meeres" }, { "fr", "La Riviera de la mer Rouge" }, { "it", "La Riviera del Mar Rosso" }, { "ru", "Ривьера Красного моря" } }, Highlights = new Dictionary<string, string> { { "en", "Beautiful beaches, great diving." }, { "de", "Schöne Strände, tolles Tauchen." }, { "fr", "Belles plages, super plongée." }, { "it", "Spiagge bellissime, ottime immersioni." }, { "ru", "Красивые пляжи, отличный дайвинг." } }, ImageUrl = "/images/hurghada.jpg", FlagEmoji = "🏖️" },
            new Destination { Id = Guid.Parse("00000000-0000-0000-0000-000000000012"), Names = new Dictionary<string, string> { { "en", "Luxor" }, { "de", "Luxor" }, { "fr", "Louxor" }, { "it", "Luxor" }, { "ru", "Луксор" } }, Descriptions = new Dictionary<string, string> { { "en", "The World's Greatest Open-Air Museum" }, { "de", "Das größte Freilichtmuseum der Welt" }, { "fr", "Le plus grand musée en plein air du monde" }, { "it", "Il più grande museo all'aperto del mondo" }, { "ru", "Величайший в мире музей под открытым небом" } }, Highlights = new Dictionary<string, string> { { "en", "Karnak Temple, Valley of Kings." }, { "de", "Karnak-Tempel, Tal der Könige." }, { "fr", "Temple de Karnak, Vallée des Rois." }, { "it", "Tempio di Karnak, Valle dei Re." }, { "ru", "Карнакский храм, Долина царей." } }, ImageUrl = "/images/luxor.jpg", FlagEmoji = "🏺" },
            new Destination { Id = Guid.Parse("00000000-0000-0000-0000-000000000013"), Names = new Dictionary<string, string> { { "en", "Cairo" }, { "de", "Kairo" }, { "fr", "Le Caire" }, { "it", "Il Cairo" }, { "ru", "Каир" } }, Descriptions = new Dictionary<string, string> { { "en", "The City of a Thousand Minarets" }, { "de", "Die Stadt der tausend Minarette" }, { "fr", "La ville aux mille minarets" }, { "it", "La città dei mille minareti" }, { "ru", "Город тысячи минаретов" } }, Highlights = new Dictionary<string, string> { { "en", "Pyramids of Giza, Egyptian Museum." }, { "de", "Pyramiden von Gizeh, Ägyptisches Museum." }, { "fr", "Pyramides de Gizeh, Musée égyptien." }, { "it", "Piramidi di Giza, Museo Egizio." }, { "ru", "Пирамиды Гизы, Египетский музей." } }, ImageUrl = "/images/cairo.jpg", FlagEmoji = "🇪🇬" },
            new Destination { Id = Guid.Parse("00000000-0000-0000-0000-000000000014"), Names = new Dictionary<string, string> { { "en", "Sharm El-Sheikh" }, { "de", "Sharm El-Sheikh" }, { "fr", "Charm el-Cheikh" }, { "it", "Sharm El-Sheikh" }, { "ru", "Шарм-эш-Шейх" } }, Descriptions = new Dictionary<string, string> { { "en", "The City of Peace" }, { "de", "Die Stadt des Friedens" }, { "fr", "La ville de la paix" }, { "it", "La città della pace" }, { "ru", "Город мира" } }, Highlights = new Dictionary<string, string> { { "en", "Naama Bay, Ras Mohammed." }, { "de", "Naama Bay, Ras Mohammed." }, { "fr", "Naama Bay, Ras Mohammed." }, { "it", "Naama Bay, Ras Mohammed." }, { "ru", "Наама Бэй, Рас Мохаммед." } }, ImageUrl = "/images/sharm.jpg", FlagEmoji = "🐠" },
            new Destination { Id = Guid.Parse("00000000-0000-0000-0000-000000000015"), Names = new Dictionary<string, string> { { "en", "Marsa Alam" }, { "de", "Marsa Alam" }, { "fr", "Marsa Alam" }, { "it", "Marsa Alam" }, { "ru", "Марса Алам" } }, Descriptions = new Dictionary<string, string> { { "en", "Diving Paradise" }, { "de", "Taucherparadies" }, { "fr", "Paradis de la plongée" }, { "it", "Paradiso delle immersioni" }, { "ru", "Рай для дайвинга" } }, Highlights = new Dictionary<string, string> { { "en", "Abu Dabbab, Dolphin House." }, { "de", "Abu Dabbab, Dolphin House." }, { "fr", "Abu Dabbab, Dolphin House." }, { "it", "Abu Dabbab, Dolphin House." }, { "ru", "Абу Даббаб, Дом Дельфинов." } }, ImageUrl = "/images/tours/a54ea17e-0023-4be6-bdb2-ea071d4f834c.JPG", FlagEmoji = "🐢" }
        };
        if (!await context.Destinations.AnyAsync())
        {
            context.Destinations.AddRange(destinations);
            await context.SaveChangesAsync();
        }

        context.ChangeTracker.Clear();

        // 3. Seed Tours
        var tours = new List<Tour>
        {
            // 1. Cairo
            new Tour {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000101"),
                Names = new Dictionary<string, string> { { "en", "Cairo: Pyramids, Sphinx, & Camel Ride" }, { "de", "Kairo: Pyramiden, Sphinx & Kamelritt" }, { "it", "Il Cairo: Piramidi, Sfinge e Cammello" }, { "fr", "Le Caire : Pyramides, Sphinx et Chameau" }, { "ru", "Каир: Пирамиды, Сфинкс и верблюд" } },
                Descriptions = new Dictionary<string, string> { { "en", "Visit the Great Pyramids of Giza, Sphinx, Valley Temple, and enjoy a Camel Ride at Giza." }, { "de", "Besuchen Sie die Großen Pyramiden von Gizeh, Sphinx, Taltempel und genießen Sie einen Kamelritt." }, { "it", "Visita le Grandi Piramidi di Giza, Sfinge, Tempio a Valle, e goditi un giro in cammello." }, { "fr", "Visitez les Grandes Pyramides de Gizeh, le Sphinx, le Temple de la Vallée et profitez d'une balade à chameau." }, { "ru", "Посетите Великие Пирамиды Гизы, Сфинкса, Долинный храм и прокатитесь на верблюде." } },
                Price = 80, StartTime = "08:00", Rating = 4.9m, ReviewCount = 1200, DestinationId = destinations[2].Id, CategoryId = categories[1].Id,
                MediaUrls = new List<string> {
                    "/images/tours/ea0bf799-cf52-49c9-ae03-00a17367deed.JPG",
                    "/images/tours/ca84c70f-b026-4ecc-800b-f2a4e0b812b7.JPG",
                    "/images/tours/9d5093fb-c73e-4c10-8740-028e25311b3c.JPG",
                    "/images/tours/ee8078ba-d656-4d4d-949d-37ff635aad50.JPG",
                    "/images/tours/7be04f61-5730-44fc-bf7e-e30d90dbf0cc.JPG"
                },
                ImageUrl = "/images/tours/ea0bf799-cf52-49c9-ae03-00a17367deed.JPG", Emoji = "🐪",
                Addons = new List<TourAddon> { new TourAddon { Names = new Dictionary<string, string> { { "en", "VIP Lunch" } }, PriceEur = 30 } }
            },
            // 2. Egyptian New Grand Museum
            new Tour {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000102"),
                Names = new Dictionary<string, string> { { "en", "Grand Egyptian Museum & Royal Treasures" }, { "de", "Großes Ägyptisches Museum" }, { "it", "Grande Museo Egizio" }, { "fr", "Grand Musée Égyptien" }, { "ru", "Большой Египетский Музей" } },
                Descriptions = new Dictionary<string, string> { { "en", "Discover the Grand Egyptian Museum (GEM) & Royal Treasures." }, { "de", "Entdecken Sie das GEM." }, { "it", "Scopri il GEM." }, { "fr", "Découvrez le GEM." }, { "ru", "Откройте для себя GEM." } },
                Price = 60, StartTime = "09:00", Rating = 4.8m, ReviewCount = 800, DestinationId = destinations[2].Id, CategoryId = categories[1].Id,
                MediaUrls = new List<string> {
                    "/images/tours/3811bc62-0273-4249-ad81-c49db48b8e39.JPG",
                    "/images/tours/27dc098b-c4ac-493f-987a-d6853b9a8033.JPG",
                    "/images/tours/01f7cc1b-ab8a-49a2-87fb-93494909eea2.JPG",
                    "/images/tours/0250f9da-c94a-4640-ba3a-7591aea4e2a0.JPG",
                    "/images/tours/2af938ca-dd0c-4e70-a81d-e3cdca92a72a.JPG",
                    "/images/tours/65bdfe96-3abe-4b65-bc08-98a50bb32f1d.JPG",
                    "/images/tours/d58786d6-0c38-480c-bcbd-6832135b4a74.JPG"
                },
                ImageUrl = "/images/tours/3811bc62-0273-4249-ad81-c49db48b8e39.JPG", Emoji = "🏛️",
                Addons = new List<TourAddon> { new TourAddon { Names = new Dictionary<string, string> { { "en", "Expert Guide" } }, PriceEur = 50 } }
            },
            // 3. Luxor
            new Tour {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000103"),
                Names = new Dictionary<string, string> { { "en", "Luxor: Karnak, Valley of the Kings, Dendera" }, { "de", "Luxor: Karnak, Tal der Könige" }, { "it", "Luxor: Karnak, Valle dei Re" }, { "fr", "Louxor: Karnak, Vallée des Rois" }, { "ru", "Луксор: Карнак, Долина царей" } },
                Descriptions = new Dictionary<string, string> { { "en", "Visit Karnak Temple, Luxor Temple, Valley of the Kings, Colossi of Memnon, and Dendera." }, { "de", "Besuchen Sie den Karnak-Tempel, Tal der Könige..." }, { "it", "Visita il Tempio di Karnak, Valle dei Re..." }, { "fr", "Visitez le temple de Karnak, Vallée des Rois..." }, { "ru", "Посетите Карнакский храм, Долину царей..." } },
                Price = 110, StartTime = "06:00", Rating = 4.9m, ReviewCount = 1500, DestinationId = destinations[1].Id, CategoryId = categories[1].Id,
                MediaUrls = new List<string> {
                    "/images/tours/1aab19b3-0bfb-4bd9-8c90-96ec7f2ce686.JPG",
                    "/images/tours/89f4aa6e-e527-472e-a33c-4c71099cbdc3.JPG",
                    "/images/tours/6c3e9e89-990e-483d-b642-5d67f27b920f.JPG",
                    "/images/tours/15438361-ff96-493e-917d-1712a7615057.JPG",
                    "/images/tours/08d4c962-b5c4-42fd-8877-a6fb1fc997ec.JPG",
                    "/images/tours/b30aad6f-74ad-4e69-a832-7d399d51193a.JPG",
                    "/images/tours/3852469b-1646-4fd6-a501-51bc01dd774b.JPG"
                },
                ImageUrl = "/images/tours/1aab19b3-0bfb-4bd9-8c90-96ec7f2ce686.JPG", Emoji = "🏺"
            },
            // 4. Sea Trip Parachute
            new Tour {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000104"),
                Names = new Dictionary<string, string> { { "en", "Hurghada Red Sea Parasailing & Boat Ride" }, { "de", "Hurghada Parasailing" }, { "it", "Hurghada Parasailing" }, { "fr", "Hurghada Parasailing" }, { "ru", "Парасейлинг в Хургаде" } },
                Descriptions = new Dictionary<string, string> { { "en", "Enjoy Hurghada Red Sea Parasailing & Boat Ride." }, { "de", "Genießen Sie Parasailing am Roten Meer." }, { "it", "Goditi il Parasailing sul Mar Rosso." }, { "fr", "Profitez du parachute ascensionnel sur la mer Rouge." }, { "ru", "Насладитесь парасейлингом на Красном море." } },
                Price = 30, StartTime = "10:00", Rating = 4.7m, ReviewCount = 600, DestinationId = destinations[0].Id, CategoryId = categories[3].Id,
                MediaUrls = new List<string> {
                    "/images/tours/cbe61e7f-87ff-4ada-b1aa-4f2c3acd92e6.JPG",
                    "/images/tours/534a275e-0dbe-48dd-b15a-bce927470ba9.JPG",
                    "/images/tours/5616cea0-2d17-48e7-9f08-69716378b9ef.JPG",
                    "/images/tours/3771ec82-c4e5-429f-bed2-9a9d4accc7f7.JPG",
                    "/images/tours/a54ea17e-0023-4be6-bdb2-ea071d4f834c.JPG"
                },
                ImageUrl = "/images/tours/cbe61e7f-87ff-4ada-b1aa-4f2c3acd92e6.JPG", Emoji = "🪂",
                Addons = new List<TourAddon> { new TourAddon { Names = new Dictionary<string, string> { { "en", "Photos" } }, PriceEur = 15 } }
            },
            // 5. Safari
            new Tour {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000105"),
                Names = new Dictionary<string, string> { { "en", "Hurghada Super Desert Quad Safari" }, { "de", "Hurghada Wüsten-Safari" }, { "it", "Safari nel Deserto di Hurghada" }, { "fr", "Safari Désert Hurghada" }, { "ru", "Сафари в пустыне Хургады" } },
                Descriptions = new Dictionary<string, string> { { "en", "ATV Convoy, Buggy Pioneer & Bedouin Dinner." }, { "de", "ATV, Buggy & Beduinen-Abendessen." }, { "it", "ATV, Buggy e cena beduina." }, { "fr", "ATV, Buggy et dîner bédouin." }, { "ru", "Квадроциклы, багги и ужин у бедуинов." } },
                Price = 40, StartTime = "14:00", Rating = 4.8m, ReviewCount = 2000, DestinationId = destinations[0].Id, CategoryId = categories[2].Id,
                MediaUrls = new List<string> {
                    "/images/tours/b468ba99-3dc2-421d-89ee-2b6f082ddc55.JPG",
                    "/images/tours/62e81a6f-643a-4c73-b61b-603ae9b5b57e.JPG",
                    "/images/tours/f36aab1f-0355-434e-aca9-de0b14e82fe5.JPG",
                    "/images/tours/7dec932e-912e-4a09-9ffd-2c724d297935.JPG",
                    "/images/tours/88c791ef-69c6-4787-b413-8385b2f53968.JPG"
                },
                ImageUrl = "/images/tours/b468ba99-3dc2-421d-89ee-2b6f082ddc55.JPG", Emoji = "🏜️",
                Addons = new List<TourAddon> { new TourAddon { Names = new Dictionary<string, string> { { "en", "Scarf" } }, PriceEur = 5 } }
            },
            // 6. Sub Marine
            new Tour {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000106"),
                Names = new Dictionary<string, string> { { "en", "Royal Seascope Semi-Submarine" }, { "de", "Semi-U-Boot" }, { "it", "Semi-Sottomarino" }, { "fr", "Semi-Sous-marin" }, { "ru", "Полуподводная лодка" } },
                Descriptions = new Dictionary<string, string> { { "en", "Coral Reef Observation Cruise." }, { "de", "Korallenriff-Beobachtungskreuzfahrt." }, { "it", "Crociera di osservazione della barriera corallina." }, { "fr", "Croisière d'observation des récifs coralliens." }, { "ru", "Круиз с наблюдением за коралловыми рифами." } },
                Price = 25, StartTime = "09:00", Rating = 4.5m, ReviewCount = 400, DestinationId = destinations[0].Id, CategoryId = categories[3].Id,
                MediaUrls = new List<string> {
                    "/images/tours/3771ec82-c4e5-429f-bed2-9a9d4accc7f7.JPG",
                    "/images/tours/5616cea0-2d17-48e7-9f08-69716378b9ef.JPG",
                    "/images/tours/534a275e-0dbe-48dd-b15a-bce927470ba9.JPG",
                    "/images/tours/cbe61e7f-87ff-4ada-b1aa-4f2c3acd92e6.JPG",
                    "/images/tours/a54ea17e-0023-4be6-bdb2-ea071d4f834c.JPG"
                },
                ImageUrl = "/images/tours/3771ec82-c4e5-429f-bed2-9a9d4accc7f7.JPG", Emoji = "🛥️"
            },
            // 7. Orange Bay Island
            new Tour {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000107"),
                Names = new Dictionary<string, string> { { "en", "Orange Bay Island VIP Cruise" }, { "de", "Orange Bay VIP-Kreuzfahrt" }, { "it", "Crociera VIP Orange Bay" }, { "fr", "Croisière VIP Orange Bay" }, { "ru", "VIP-круиз на остров Оранж Бей" } },
                Descriptions = new Dictionary<string, string> { { "en", "Orange Bay Island VIP Cruise & Beach Paradise." }, { "de", "Orange Bay VIP-Kreuzfahrt & Strandparadies." }, { "it", "Crociera VIP Orange Bay & Paradiso sulla Spiaggia." }, { "fr", "Croisière VIP Orange Bay & Paradis sur la Plage." }, { "ru", "VIP-круиз на остров Оранж Бей и рай на пляже." } },
                Price = 45, StartTime = "08:30", Rating = 4.9m, ReviewCount = 1100, DestinationId = destinations[0].Id, CategoryId = categories[3].Id,
                MediaUrls = new List<string> {
                    "/images/tours/5616cea0-2d17-48e7-9f08-69716378b9ef.JPG",
                    "/images/tours/3771ec82-c4e5-429f-bed2-9a9d4accc7f7.JPG",
                    "/images/tours/cbe61e7f-87ff-4ada-b1aa-4f2c3acd92e6.JPG",
                    "/images/tours/534a275e-0dbe-48dd-b15a-bce927470ba9.JPG",
                    "/images/tours/a54ea17e-0023-4be6-bdb2-ea071d4f834c.JPG"
                },
                ImageUrl = "/images/tours/5616cea0-2d17-48e7-9f08-69716378b9ef.JPG", Emoji = "🏝️",
                Addons = new List<TourAddon> { new TourAddon { Names = new Dictionary<string, string> { { "en", "Seafood Lunch" } }, PriceEur = 20 } }
            },
            // 8. Dolphin House Boat Trip
            new Tour {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000108"),
                Names = new Dictionary<string, string> { { "en", "Dolphin House Snorkeling Yacht Cruise" }, { "de", "Dolphin House Schnorcheln" }, { "it", "Snorkeling al Dolphin House" }, { "fr", "Snorkeling Dolphin House" }, { "ru", "Снорклинг с дельфинами" } },
                Descriptions = new Dictionary<string, string> { { "en", "Dolphin House Snorkeling Yacht Cruise." }, { "de", "Dolphin House Schnorchel-Yacht-Kreuzfahrt." }, { "it", "Crociera in yacht per snorkeling al Dolphin House." }, { "fr", "Croisière en yacht pour snorkeling au Dolphin House." }, { "ru", "Круиз на яхте со снорклингом к дельфинам." } },
                Price = 35, StartTime = "08:00", Rating = 4.6m, ReviewCount = 850, DestinationId = destinations[0].Id, CategoryId = categories[3].Id,
                MediaUrls = new List<string> {
                    "/images/tours/534a275e-0dbe-48dd-b15a-bce927470ba9.JPG",
                    "/images/tours/5616cea0-2d17-48e7-9f08-69716378b9ef.JPG",
                    "/images/tours/3771ec82-c4e5-429f-bed2-9a9d4accc7f7.JPG",
                    "/images/tours/cbe61e7f-87ff-4ada-b1aa-4f2c3acd92e6.JPG",
                    "/images/tours/a54ea17e-0023-4be6-bdb2-ea071d4f834c.JPG"
                },
                ImageUrl = "/images/tours/534a275e-0dbe-48dd-b15a-bce927470ba9.JPG", Emoji = "🐬",
                Addons = new List<TourAddon> { new TourAddon { Names = new Dictionary<string, string> { { "en", "GoPro Rental" } }, PriceEur = 25 } }
            },
            // 9. Luxor Parachute
            new Tour {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000109"),
                Names = new Dictionary<string, string> { { "en", "Luxor Sunrise Hot Air Balloon" }, { "de", "Luxor Heißluftballon" }, { "it", "Mongolfiera a Luxor" }, { "fr", "Montgolfière à Louxor" }, { "ru", "Воздушный шар в Луксоре" } },
                Descriptions = new Dictionary<string, string> { { "en", "Luxor Sunrise Hot Air Balloon flight over West Bank." }, { "de", "Heißluftballonflug bei Sonnenaufgang über das Westufer." }, { "it", "Volo in mongolfiera all'alba sopra la riva occidentale." }, { "fr", "Vol en montgolfière au lever du soleil sur la rive ouest." }, { "ru", "Полет на воздушном шаре на рассвете над Западным берегом." } },
                Price = 85, StartTime = "04:30", Rating = 5.0m, ReviewCount = 3000, DestinationId = destinations[1].Id, CategoryId = categories[2].Id,
                MediaUrls = new List<string> {
                    "/images/tours/89f4aa6e-e527-472e-a33c-4c71099cbdc3.JPG",
                    "/images/tours/1aab19b3-0bfb-4bd9-8c90-96ec7f2ce686.JPG",
                    "/images/tours/6c3e9e89-990e-483d-b642-5d67f27b920f.JPG",
                    "/images/tours/15438361-ff96-493e-917d-1712a7615057.JPG",
                    "/images/tours/08d4c962-b5c4-42fd-8877-a6fb1fc997ec.JPG"
                },
                ImageUrl = "/images/tours/89f4aa6e-e527-472e-a33c-4c71099cbdc3.JPG", Emoji = "🎈"
            },
            // 10. Abu Simbel
            new Tour {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000110"),
                Names = new Dictionary<string, string> { { "en", "Abu Simbel Sun Temples & Philae" }, { "de", "Abu Simbel & Philae" }, { "it", "Abu Simbel e Philae" }, { "fr", "Abou Simbel et Philae" }, { "ru", "Абу-Симбел и Филе" } },
                Descriptions = new Dictionary<string, string> { { "en", "Abu Simbel Sun Temples of Ramses II & Philae Nile Island in Aswan." }, { "de", "Sonnentempel von Abu Simbel & Philae Nilinsel in Assuan." }, { "it", "Templi del sole di Abu Simbel & Isola del Nilo di Philae ad Assuan." }, { "fr", "Temples du soleil d'Abou Simbel et île du Nil de Philae à Assouan." }, { "ru", "Храмы солнца Абу-Симбел и остров Филе на Ниле в Асуане." } },
                Price = 130, StartTime = "05:00", Rating = 4.9m, ReviewCount = 900, DestinationId = destinations[1].Id, CategoryId = categories[1].Id,
                MediaUrls = new List<string> {
                    "/images/tours/099c10ac-0473-43a5-9b75-86c0b110d627.JPG",
                    "/images/tours/75377b99-aea2-4f05-b14a-899c761d09b3.JPG",
                    "/images/tours/1aab19b3-0bfb-4bd9-8c90-96ec7f2ce686.JPG",
                    "/images/tours/89f4aa6e-e527-472e-a33c-4c71099cbdc3.JPG",
                    "/images/tours/6c3e9e89-990e-483d-b642-5d67f27b920f.JPG"
                },
                ImageUrl = "/images/tours/099c10ac-0473-43a5-9b75-86c0b110d627.JPG", Emoji = "🛕"
            },
            // 11. Hula Hula Island Boat Trip
            new Tour {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000111"),
                Names = new Dictionary<string, string> { { "en", "Hula Hula Island Luxury Day Tour" }, { "de", "Hula Hula Insel Luxus-Tour" }, { "it", "Tour di Lusso all'Isola di Hula Hula" }, { "fr", "Tour de Luxe de l'Île de Hula Hula" }, { "ru", "Роскошный тур на остров Хула-Хула" } },
                Descriptions = new Dictionary<string, string> { { "en", "Hula Hula Island Luxury Day Tour." }, { "de", "Hula Hula Insel Luxus-Tagestour." }, { "it", "Tour di lusso di un giorno all'isola di Hula Hula." }, { "fr", "Tournée de luxe d'une journée sur l'île de Hula Hula." }, { "ru", "Роскошный дневной тур на остров Хула-Хула." } },
                Price = 55, StartTime = "08:30", Rating = 4.8m, ReviewCount = 500, DestinationId = destinations[0].Id, CategoryId = categories[3].Id,
                MediaUrls = new List<string> {
                    "/images/tours/5616cea0-2d17-48e7-9f08-69716378b9ef.JPG",
                    "/images/tours/3771ec82-c4e5-429f-bed2-9a9d4accc7f7.JPG",
                    "/images/tours/cbe61e7f-87ff-4ada-b1aa-4f2c3acd92e6.JPG",
                    "/images/tours/534a275e-0dbe-48dd-b15a-bce927470ba9.JPG",
                    "/images/tours/a54ea17e-0023-4be6-bdb2-ea071d4f834c.JPG"
                },
                ImageUrl = "/images/tours/5616cea0-2d17-48e7-9f08-69716378b9ef.JPG", Emoji = "🥥"
            }
        };

        // Assign Suppliers dynamically to seeded tours
        foreach (var tour in tours)
        {
            tour.SupplierId = Guid.Parse("00000000-0000-0000-0000-000000000081"); // Red Sea as default
            tour.SupplierPercentage = 15;

            // Populate Rich Tabs dynamically
            tour.Packages = new List<TourPackage>
            {
                new TourPackage
                {
                    Id = Guid.NewGuid(),
                    Titles = new Dictionary<string, string> { { "en", "Standard Package" }, { "de", "Standardpaket" }, { "fr", "Forfait Standard" }, { "it", "Pacchetto Standard" }, { "ru", "Стандартный пакет" } },
                    Descriptions = new Dictionary<string, string> { { "en", "Includes all basic amenities" }, { "de", "Inklusive aller grundlegenden Annehmlichkeiten" }, { "fr", "Comprend toutes les commodités de base" }, { "it", "Include tutti i servizi di base" }, { "ru", "Включает все базовые удобства" } },
                    Price = tour.Price,
                    Badge = "Popular",
                    Features = new Dictionary<string, string> { { "en", "Guided tour, Transportation" }, { "de", "Führung, Transport" }, { "fr", "Visite guidée, Transport" }, { "it", "Visita guidata, Trasporto" }, { "ru", "Экскурсия, Транспорт" } }
                }
            };
            
            tour.Highlights = new Dictionary<string, string>
            {
                { "en", "Experience the best of the region; Memorable moments guaranteed; Expert local guides" }, { "de", "Erleben Sie das Beste der Region; Unvergessliche Momente garantiert; Kompetente lokale Führer" }, { "fr", "Découvrez le meilleur de la région ; Moments mémorables garantis ; Guides locaux experts" }, { "it", "Vivi il meglio della regione; Momenti indimenticabili garantiti; Guide locali esperte" }, { "ru", "Почувствуйте лучшее в регионе; Незабываемые моменты гарантированы; Опытные местные гиды" }
            };
            
            tour.Itinerary = GenerateDefaultItinerary();

            tour.Inclusions = new List<TourInclusion>
            {
                new TourInclusion { Names = new Dictionary<string, string> { { "en", "Hotel pickup and drop-off" }, { "de", "Hotelabholung und -rückgabe" }, { "fr", "Prise en charge et retour à l'hôtel" }, { "it", "Prelievo e rientro in hotel" }, { "ru", "Трансфер из отеля и обратно" } } },
                new TourInclusion { Names = new Dictionary<string, string> { { "en", "Professional guide" }, { "de", "Professioneller Guide" }, { "fr", "Guide professionnel" }, { "it", "Guida professionale" }, { "ru", "Профессиональный гид" } } }
            };

            tour.Exclusions = new List<TourInclusion>
            {
                new TourInclusion { Names = new Dictionary<string, string> { { "en", "Personal expenses" }, { "de", "Persönliche Ausgaben" }, { "fr", "Dépenses personnelles" }, { "it", "Spese personali" }, { "ru", "Личные расходы" } } },
                new TourInclusion { Names = new Dictionary<string, string> { { "en", "Gratuities" }, { "de", "Trinkgelder" }, { "fr", "Pourboires" }, { "it", "Mance" }, { "ru", "Чаевые" } } }
            };

            tour.ImportantInformation = new ImportantInfo
            {
                WhatToBring = new Dictionary<string, string> { { "en", "Comfortable shoes, Camera, Sunscreen" }, { "de", "Bequeme Schuhe, Kamera, Sonnencreme" }, { "fr", "Chaussures confortables, appareil photo, crème solaire" }, { "it", "Scarpe comode, fotocamera, crema solare" }, { "ru", "Удобная обувь, камера, солнцезащитный крем" } },
                NotSuitableFor = new Dictionary<string, string> { { "en", "People with mobility impairments" }, { "de", "Menschen mit Mobilitätseinschränkungen" }, { "fr", "Personnes à mobilité réduite" }, { "it", "Persone con disabilità motorie" }, { "ru", "Люди с ограниченными физическими возможностями" } },
                Notes = new Dictionary<string, string> { { "en", "Subject to favorable weather conditions" }, { "de", "Abhängig von günstigen Wetterbedingungen" }, { "fr", "Sous réserve de conditions météorologiques favorables" }, { "it", "Soggetto a condizioni meteorologiche favorevoli" }, { "ru", "При благоприятных погодных условиях" } }
            };

            tour.Faqs = GenerateDefaultFaqs();

            tour.Addons = new List<TourAddon>
            {
                new TourAddon
                {
                    Id = Guid.NewGuid(),
                    Names = new Dictionary<string, string> { { "en", "VIP Hotel Transfer" }, { "de", "VIP-Hoteltransfer" }, { "fr", "Transfert VIP Hôtel" }, { "it", "Trasferimento VIP in Hotel" }, { "ru", "VIP-трансфер из отеля" } },
                    Descriptions = new Dictionary<string, string> { { "en", "Private air-conditioned vehicle" }, { "de", "Privates klimatisiertes Fahrzeug" }, { "fr", "Véhicule privé climatisé" }, { "it", "Veicolo privato climatizzato" }, { "ru", "Частный автомобиль с кондиционером" } },
                    PriceEur = 25,
                    IsPerPerson = false,
                    Icon = "🚗",
                    Category = "Transport"
                },
                new TourAddon
                {
                    Id = Guid.NewGuid(),
                    Names = new Dictionary<string, string> { { "en", "Professional Photo/Video Package" }, { "de", "Professionelles Foto-/Videopaket" }, { "fr", "Forfait Photo/Vidéo Professionnel" }, { "it", "Pacchetto Foto/Video Professionale" }, { "ru", "Профессиональный пакет фото/видео" } },
                    Descriptions = new Dictionary<string, string> { { "en", "High-res digital photos and drone footage" }, { "de", "Hochauflösende Digitalfotos und Drohnenaufnahmen" }, { "fr", "Photos numériques haute résolution et images de drone" }, { "it", "Foto digitali ad alta risoluzione e riprese con drone" }, { "ru", "Цифровые фотографии высокого разрешения и съемка с дрона" } },
                    PriceEur = 40,
                    IsPerPerson = false,
                    Icon = "📸",
                    Category = "Photography"
                }
            };

            tour.Media = (tour.MediaUrls ?? new List<string>()).Select(url => new TourMedia
            {
                Url = url,
                Captions = new Dictionary<string, string> { { "en", tour.Names.GetValueOrDefault("en", "Tour Experience") }, { "de", tour.Names.GetValueOrDefault("de", "Tour Erlebnis") }, { "fr", tour.Names.GetValueOrDefault("fr", "Expérience de Tour") }, { "it", tour.Names.GetValueOrDefault("it", "Esperienza di Tour") }, { "ru", tour.Names.GetValueOrDefault("ru", "Тур Опыт") } }
            }).ToList();
        }

        context.Tours.AddRange(tours);
        await context.SaveChangesAsync();
    }

    private static List<TourItinerary> GenerateDefaultItinerary()
    {
        // Default itinerary for all
        return new List<TourItinerary>
        {
            new TourItinerary { Titles = new Dictionary<string, string> { { "en", "Pickup & Departure" }, { "de", "Abholung & Abfahrt" }, { "fr", "Prise en charge et départ" }, { "it", "Prelievo e partenza" }, { "ru", "Встреча и отправление" } }, Descriptions = new Dictionary<string, string> { { "en", "Meet at the hotel lobby and head to the destination." }, { "de", "Treffen Sie sich in der Hotellobby und fahren Sie zum Ziel." }, { "fr", "Rendez-vous dans le hall de l'hôtel et dirigez-vous vers la destination." }, { "it", "Incontro nella hall dell'hotel e partenza per la destinazione." }, { "ru", "Встреча в холле отеля и отправление в пункт назначения." } } },
            new TourItinerary { Titles = new Dictionary<string, string> { { "en", "Main Activity" }, { "de", "Hauptaktivität" }, { "fr", "Activité Principale" }, { "it", "Attività Principale" }, { "ru", "Основная активность" } }, Descriptions = new Dictionary<string, string> { { "en", "Enjoy the primary activity of the tour." }, { "de", "Genießen Sie die Hauptaktivität der Tour." }, { "fr", "Profitez de l'activité principale de la visite." }, { "it", "Goditi l'attività principale del tour." }, { "ru", "Наслаждайтесь основной деятельностью тура." } } },
            new TourItinerary { Titles = new Dictionary<string, string> { { "en", "Return Journey" }, { "de", "Rückfahrt" }, { "fr", "Voyage de retour" }, { "it", "Viaggio di Ritorno" }, { "ru", "Обратный путь" } }, Descriptions = new Dictionary<string, string> { { "en", "Head back to your hotel." }, { "de", "Fahren Sie zurück zu Ihrem Hotel." }, { "fr", "Retournez à votre hôtel." }, { "it", "Torna al tuo hotel." }, { "ru", "Возвращение в отель." } } }
        };
    }

    private static List<TourFaq> GenerateDefaultFaqs()
    {
        return new List<TourFaq>
        {
            new TourFaq
            {
                Questions = new Dictionary<string, string> { { "en", "How do I receive my Booking Confirmation?" }, { "de", "Wie erhalte ich meine Buchungsbestätigung?" }, { "fr", "Comment puis-je recevoir ma confirmation de réservation?" }, { "it", "Come ricevo la mia conferma di prenotazione?" }, { "ru", "Как мне получить подтверждение бронирования?" } },
                Answers = new Dictionary<string, string> { { "en", "You will receive an instant WhatsApp voucher and an email confirmation immediately upon reservation." }, { "de", "Sie erhalten sofort nach der Reservierung einen WhatsApp-Gutschein und eine E-Mail-Bestätigung." }, { "fr", "Vous recevrez un bon WhatsApp instantané et une confirmation par e-mail immédiatement après la réservation." }, { "it", "Riceverai un voucher WhatsApp istantaneo e una conferma via e-mail immediatamente dopo la prenotazione." }, { "ru", "Вы получите мгновенный ваучер WhatsApp и подтверждение по электронной почте сразу после бронирования." } }
            },
            new TourFaq
            {
                Questions = new Dictionary<string, string> { { "en", "What Payment Options are available?" }, { "de", "Welche Zahlungsoptionen stehen zur Verfügung?" }, { "fr", "Quelles sont les options de paiement disponibles?" }, { "it", "Quali opzioni di pagamento sono disponibili?" }, { "ru", "Какие варианты оплаты доступны?" } },
                Answers = new Dictionary<string, string> { { "en", "We offer secure online card payment during booking, or you can choose our flexible Pay-on-Pickup option." }, { "de", "Wir bieten eine sichere Online-Kartenzahlung während der Buchung oder Sie können unsere flexible Pay-on-Pickup-Option wählen." }, { "fr", "Nous proposons un paiement par carte en ligne sécurisé lors de la réservation, ou vous pouvez choisir notre option de paiement flexible lors de la prise en charge." }, { "it", "Offriamo il pagamento sicuro con carta online durante la prenotazione, oppure puoi scegliere la nostra flessibile opzione Pay-on-Pickup." }, { "ru", "Мы предлагаем безопасную оплату картой онлайн во время бронирования, или вы можете выбрать гибкий вариант оплаты при получении." } }
            },
            new TourFaq
            {
                Questions = new Dictionary<string, string> { { "en", "What is the Cancellation Policy?" }, { "de", "Was ist die Stornierungsrichtlinie?" }, { "fr", "Quelle est la politique d'annulation?" }, { "it", "Qual è la politica di cancellazione?" }, { "ru", "Какова политика отмены?" } },
                Answers = new Dictionary<string, string> { { "en", "Free cancellation up to 72 hours before departure. A 25% penalty applies if canceled within 48 hours, and a 50% penalty if canceled under 24 hours." }, { "de", "Kostenlose Stornierung bis zu 72 Stunden vor der Abreise. Bei Stornierung innerhalb von 48 Stunden wird eine Gebühr von 25% erhoben, bei Stornierung innerhalb von 24 Stunden eine Gebühr von 50%." }, { "fr", "Annulation gratuite jusqu'à 72 heures avant le départ. Une pénalité de 25% s'applique en cas d'annulation dans les 48 heures, et une pénalité de 50% en cas d'annulation à moins de 24 heures." }, { "it", "Cancellazione gratuita fino a 72 ore prima della partenza. Si applica una penale del 25% in caso di cancellazione entro 48 ore e una penale del 50% in caso di cancellazione entro 24 ore." }, { "ru", "Бесплатная отмена за 72 часа до отправления. Штраф 25% применяется при отмене за 48 часов и штраф 50% при отмене менее чем за 24 часа." } }
            },
            new TourFaq
            {
                Questions = new Dictionary<string, string> { { "en", "How does Hotel Pickup work?" }, { "de", "Wie funktioniert die Abholung vom Hotel?" }, { "fr", "Comment fonctionne la prise en charge à l'hôtel?" }, { "it", "Come funziona il prelievo in hotel?" }, { "ru", "Как работает встреча в отеле?" } },
                Answers = new Dictionary<string, string> { { "en", "Please wait in your hotel lobby at the scheduled pickup time. Our guide will verify your room number upon arrival." }, { "de", "Bitte warten Sie zum vereinbarten Abholzeitpunkt in Ihrer Hotellobby. Unser Guide wird Ihre Zimmernummer bei der Ankunft überprüfen." }, { "fr", "Veuillez patienter dans le hall de votre hôtel à l'heure de prise en charge prévue. Notre guide vérifiera votre numéro de chambre à son arrivée." }, { "it", "Attendi nella hall dell'hotel all'orario di prelievo programmato. La nostra guida verificherà il numero della tua camera all'arrivo." }, { "ru", "Пожалуйста, подождите в холле вашего отеля в назначенное время встречи. Наш гид проверит номер вашей комнаты по прибытии." } }
            },
            new TourFaq
            {
                Questions = new Dictionary<string, string> { { "en", "Why do I need to upload my Passport/ID?" }, { "de", "Warum muss ich meinen Pass/Ausweis hochladen?" }, { "fr", "Pourquoi dois-je télécharger mon passeport / carte d'identité?" }, { "it", "Perché devo caricare il mio passaporto/carta d'identità?" }, { "ru", "Зачем мне нужно загружать свой паспорт/удостоверение личности?" } },
                Answers = new Dictionary<string, string> { { "en", "Mandatory Coast Guard and Tourism Police permits require passport or ID copies for all maritime and desert excursions to ensure your safety and compliance with local laws." }, { "de", "Zwingende Genehmigungen der Küstenwache und Tourismuspolizei erfordern Pass- oder Ausweiskopien für alle Meeres- und Wüstenausflüge, um Ihre Sicherheit und Einhaltung lokaler Gesetze zu gewährleisten." }, { "fr", "Les permis obligatoires de la Garde côtière et de la police du tourisme exigent des copies de passeport ou de carte d'identité pour toutes les excursions maritimes et dans le désert afin d'assurer votre sécurité et votre conformité avec les lois locales." }, { "it", "I permessi obbligatori della Guardia Costiera e della Polizia Turistica richiedono copie del passaporto o del documento d'identità per tutte le escursioni marittime e nel deserto per garantire la tua sicurezza e il rispetto delle leggi locali." }, { "ru", "Обязательные разрешения Береговой охраны и Туристической полиции требуют копии паспорта или удостоверения личности для всех морских и пустынных экскурсий для обеспечения вашей безопасности и соблюдения местных законов." } }
            }
        };
    }
}
