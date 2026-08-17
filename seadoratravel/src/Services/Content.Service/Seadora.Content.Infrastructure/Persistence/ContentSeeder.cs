using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Seadora.Content.Domain.Entities;
using System.Linq;

namespace Seadora.Content.Infrastructure.Persistence;

public static class ContentSeeder
{
    public static async Task SeedAsync(ContentDbContext context)
    {
        await context.Database.MigrateAsync();

        if (await context.Tours.AnyAsync())
        {
            context.Tours.RemoveRange(context.Tours);
            await context.SaveChangesAsync();
        }
        if (await context.Destinations.AnyAsync())
        {
            context.Destinations.RemoveRange(context.Destinations);
            await context.SaveChangesAsync();
        }
        if (await context.Categories.AnyAsync())
        {
            context.Categories.RemoveRange(context.Categories);
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
                    { "en", "Diving & Water Sports" }, { "ar", "غوص ورياضات مائية" }, { "fr", "Plongée & Sports Nautiques" }, { "de", "Tauchen & Wassersport" }, { "it", "Immersioni e Sport Acquatici" }, { "ru", "Дайвинг и водные виды спорта" } 
                },
                Icon = "🤿"
            },
            new Category { 
                Id = Guid.Parse("00000000-0000-0000-0000-000000000002"), 
                Names = new Dictionary<string, string> { 
                    { "en", "Culture & History" }, { "ar", "ثقافة وتاريخ" }, { "fr", "Culture & Histoire" }, { "de", "Kultur & Geschichte" }, { "it", "Cultura e Storia" }, { "ru", "Культура и история" } 
                },
                Icon = "🏛️"
            },
            new Category { 
                Id = Guid.Parse("00000000-0000-0000-0000-000000000003"), 
                Names = new Dictionary<string, string> { 
                    { "en", "Safari & Adventure" }, { "ar", "سفاري ومغامرات" }, { "fr", "Safari & Aventure" }, { "de", "Safari & Abenteuer" }, { "it", "Safari e Avventura" }, { "ru", "Сафари и приключения" } 
                },
                Icon = "🏜️"
            },
            new Category { 
                Id = Guid.Parse("00000000-0000-0000-0000-000000000004"), 
                Names = new Dictionary<string, string> { 
                    { "en", "Boat & Sea Trips" }, { "ar", "رحلات بحرية وقوارب" }, { "fr", "Excursions en bateau" }, { "de", "Boot- & Seefahrten" }, { "it", "Gite in Barca e Mare" }, { "ru", "Морские прогулки" } 
                },
                Icon = "🛳️"
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
            new Destination { Id = Guid.Parse("00000000-0000-0000-0000-000000000011"), Names = new Dictionary<string, string> { { "en", "Hurghada" } }, Descriptions = new Dictionary<string, string> { { "en", "The Red Sea Riviera" } }, ImageUrl = "/images/tours/5616cea0-2d17-48e7-9f08-69716378b9ef.JPG", Flag = "🌊" },
            new Destination { Id = Guid.Parse("00000000-0000-0000-0000-000000000012"), Names = new Dictionary<string, string> { { "en", "Luxor" } }, Descriptions = new Dictionary<string, string> { { "en", "The World's Greatest Open-Air Museum" } }, ImageUrl = "/images/tours/1aab19b3-0bfb-4bd9-8c90-96ec7f2ce686.JPG", Flag = "🏺" },
            new Destination { Id = Guid.Parse("00000000-0000-0000-0000-000000000013"), Names = new Dictionary<string, string> { { "en", "Cairo" } }, Descriptions = new Dictionary<string, string> { { "en", "The City of a Thousand Minarets" } }, ImageUrl = "/images/tours/ea0bf799-cf52-49c9-ae03-00a17367deed.JPG", Flag = "🏛️" },
            new Destination { Id = Guid.Parse("00000000-0000-0000-0000-000000000014"), Names = new Dictionary<string, string> { { "en", "Sharm El-Sheikh" } }, Descriptions = new Dictionary<string, string> { { "en", "The City of Peace" } }, ImageUrl = "/images/tours/099c10ac-0473-43a5-9b75-86c0b110d627.JPG", Flag = "🌴" }
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
                    Titles = new Dictionary<string, string> { { "en", "Standard Package" } },
                    Descriptions = new Dictionary<string, string> { { "en", "Includes all basic amenities" } },
                    Price = tour.Price,
                    Badge = "Popular",
                    Features = new Dictionary<string, List<string>> { { "en", new List<string> { "Guided tour", "Transportation" } } },
                    Inclusions = new List<TourInclusion> {
                        new TourInclusion { Names = new Dictionary<string, string> { { "en", "Lunch" } } },
                        new TourInclusion { Names = new Dictionary<string, string> { { "en", "Snorkeling Equipment" } } }
                    },
                    Exclusions = new List<TourInclusion> {
                        new TourInclusion { Names = new Dictionary<string, string> { { "en", "National Park Fee" } } }
                    }
                }
            };
            
            tour.Highlights = new Dictionary<string, List<string>>
            {
                { "en", new List<string> { "Experience the best of the region", "Memorable moments guaranteed", "Expert local guides" } }
            };
            
            tour.Itinerary = GetItineraryForTour(tour.Id);

            tour.Inclusions = new Dictionary<string, List<string>>
            {
                { "en", tour.Includes != null && tour.Includes.Any() ? tour.Includes : new List<string> { "Hotel pickup and drop-off", "Professional guide" } }
            };

            tour.Exclusions = new Dictionary<string, List<string>>
            {
                { "en", new List<string> { "Personal expenses", "Gratuities", "Meals not mentioned" } }
            };

            tour.ImportantInformation = new ImportantInfo
            {
                WhatToBring = new Dictionary<string, List<string>> { { "en", new List<string> { "Comfortable shoes", "Camera", "Sunscreen" } } },
                NotSuitableFor = new Dictionary<string, List<string>> { { "en", new List<string> { "People with mobility impairments" } } },
                Notes = new Dictionary<string, List<string>> { { "en", new List<string> { "Subject to favorable weather conditions" } } }
            };

            tour.Faqs = GetStandardFaqs();
        }

        context.Tours.AddRange(tours);
        await context.SaveChangesAsync();
    }

    private static List<TourItinerary> GenerateDefaultItinerary()
    {
        // Default itinerary for all
        return new List<TourItinerary>
        {
            new TourItinerary { Time = "08:00 AM", Titles = new Dictionary<string, string> { { "en", "Pickup & Departure" } }, Descriptions = new Dictionary<string, string> { { "en", "Meet at the hotel lobby and head to the destination." } } },
            new TourItinerary { Time = "10:30 AM", Titles = new Dictionary<string, string> { { "en", "Main Activity" } }, Descriptions = new Dictionary<string, string> { { "en", "Enjoy the primary activity of the tour." } } },
            new TourItinerary { Time = "02:00 PM", Titles = new Dictionary<string, string> { { "en", "Return Journey" } }, Descriptions = new Dictionary<string, string> { { "en", "Head back to your hotel." } } }
        };
    }

    private static List<TourFaq> GenerateDefaultFaqs()
    {
        return new List<TourFaq>
        {
            new TourFaq
            {
                Questions = new Dictionary<string, string> { { "en", "How do I receive my Booking Confirmation?" } },
                Answers = new Dictionary<string, string> { { "en", "You will receive an instant WhatsApp voucher and an email confirmation immediately upon reservation." } }
            },
            new TourFaq
            {
                Questions = new Dictionary<string, string> { { "en", "What Payment Options are available?" } },
                Answers = new Dictionary<string, string> { { "en", "We offer secure online card payment during booking, or you can choose our flexible Pay-on-Pickup option." } }
            },
            new TourFaq
            {
                Questions = new Dictionary<string, string> { { "en", "What is the Cancellation Policy?" } },
                Answers = new Dictionary<string, string> { { "en", "Free cancellation up to 72 hours before departure. A 25% penalty applies if canceled within 48 hours, and a 50% penalty if canceled under 24 hours." } }
            },
            new TourFaq
            {
                Questions = new Dictionary<string, string> { { "en", "How does Hotel Pickup work?" } },
                Answers = new Dictionary<string, string> { { "en", "Please wait in your hotel lobby at the scheduled pickup time. Our guide will verify your room number upon arrival." } }
            },
            new TourFaq
            {
                Questions = new Dictionary<string, string> { { "en", "Why do I need to upload my Passport/ID?" } },
                Answers = new Dictionary<string, string> { { "en", "Mandatory Coast Guard and Tourism Police permits require passport or ID copies for all maritime and desert excursions to ensure your safety and compliance with local laws." } }
            }
        };
    }
}
