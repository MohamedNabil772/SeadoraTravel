using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Seadora.Content.Domain.Entities;

namespace Seadora.Content.Infrastructure.Persistence;

public static class ContentSeeder
{
    public static async Task SeedAsync(ContentDbContext context)
    {
        await context.Database.EnsureCreatedAsync();

        // ponytail: seed only when empty so admin edits to categories/tours/destinations
        // survive redeploys. Schema changes need EF migrations (none yet); add when the model changes.
        if (await context.Categories.AnyAsync()) return;

        // 1. Seed Categories
        var categories = new List<Category>
        {
            new Category { 
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"), 
                Names = new Dictionary<string, string> { 
                    { "en", "Sea & Diving" }, { "ru", "Море и дайвинг" }, { "de", "Meer & Tauchen" }, { "it", "Mare e immersioni" }, { "fr", "Mer et plongée" } 
                },
                Icon = "🤿"
            },
            new Category { 
                Id = Guid.Parse("00000000-0000-0000-0000-000000000002"), 
                Names = new Dictionary<string, string> { 
                    { "en", "Culture & History" }, { "ru", "Культура и история" }, { "de", "Kultur & Geschichte" }, { "it", "Cultura e storia" }, { "fr", "Culture et histoire" } 
                },
                Icon = "🏛️"
            },
            new Category { 
                Id = Guid.Parse("00000000-0000-0000-0000-000000000003"), 
                Names = new Dictionary<string, string> { 
                    { "en", "Safari & Adventure" }, { "ru", "Сафари и приключения" }, { "de", "Safari & Abenteuer" }, { "it", "Safari e avventura" }, { "fr", "Safari et aventure" } 
                },
                Icon = "🏜️"
            }
        };
        context.Categories.AddRange(categories);

        // 2. Seed Destinations
        var destinations = new List<Destination>
        {
            new Destination { Id = Guid.Parse("00000000-0000-0000-0000-000000000011"), Names = new Dictionary<string, string> { { "en", "Hurghada" } }, Descriptions = new Dictionary<string, string> { { "en", "The Red Sea Riviera" } }, ImageUrl = "https://images.unsplash.com/photo-1539712232497-5813dec4431d", Flag = "🌊" },
            new Destination { Id = Guid.Parse("00000000-0000-0000-0000-000000000012"), Names = new Dictionary<string, string> { { "en", "Luxor" } }, Descriptions = new Dictionary<string, string> { { "en", "The World's Greatest Open-Air Museum" } }, ImageUrl = "https://images.unsplash.com/photo-1572252017416-224447c73410", Flag = "🏺" },
            new Destination { Id = Guid.Parse("00000000-0000-0000-0000-000000000013"), Names = new Dictionary<string, string> { { "en", "Cairo" } }, Descriptions = new Dictionary<string, string> { { "en", "The City of a Thousand Minarets" } }, ImageUrl = "https://images.unsplash.com/photo-1503177119275-0aa32b3a9368", Flag = "🏛️" },
            new Destination { Id = Guid.Parse("00000000-0000-0000-0000-000000000014"), Names = new Dictionary<string, string> { { "en", "Sharm El-Sheikh" } }, Descriptions = new Dictionary<string, string> { { "en", "The City of Peace" } }, ImageUrl = "https://images.unsplash.com/photo-1544551763-46a013bb70d5", Flag = "🌴" }
        };
        context.Destinations.AddRange(destinations);

        // 3. Seed Tours
        var tours = new List<Tour>
        {
            // 1. Snorkeling Safari – Orange Bay
            new Tour { 
                Id = Guid.Parse("00000000-0000-0000-0000-000000000101"), 
                Names = new Dictionary<string, string> { 
                    { "en", "Snorkeling Safari – Orange Bay" },
                    { "de", "Schnorchel-Safari – Orange Bay" },
                    { "it", "Safari di snorkeling – Orange Bay" },
                    { "fr", "Safari snorkeling – Orange Bay" },
                    { "ru", "Снорклинг-сафари – Orange Bay" }
                }, 
                Descriptions = new Dictionary<string, string> { 
                    { "en", "Sail to the stunning Orange Bay Island, snorkel among vibrant coral reefs, enjoy a BBQ lunch on a pristine beach." },
                    { "de", "Fahrt zur atemberaubenden Orange Bay Insel, Schnorcheln zwischen lebhaften Korallenriffen, BBQ-Mittagessen am Strand." },
                    { "it", "Vela verso la splendida Orange Bay, snorkeling tra barriere coralline vivaci, pranzo BBQ su una spiaggia incontaminata." },
                    { "fr", "Naviguez vers Orange Bay, snorkeling parmi les récifs coralliens, déjeuner BBQ su une plage immaculée." },
                    { "ru", "Плавание к острову Orange Bay, снорклинг среди кораллов, барбекю-обед на пляже." }
                },
                Price = 25, 
                Duration = "fullDay",
                Includes = new List<string> { "🚌 Transfer", "🥗 Lunch", "🤿 Equipment" },
                ImageUrl = "https://images.unsplash.com/photo-1544551763-46a013bb70d5",
                Emoji = "🤿",
                BgGradient = "linear-gradient(135deg,#063a5c,#1a9b8a)",
                Badge = "⭐ BESTSELLER",
                DestinationId = destinations[0].Id,
                CategoryId = categories[0].Id
            },
            // 2. Pyramids & Cairo Explorer
            new Tour { 
                Id = Guid.Parse("00000000-0000-0000-0000-000000000102"), 
                Names = new Dictionary<string, string> { 
                    { "en", "Pyramids & Cairo Explorer" },
                    { "de", "Pyramiden & Kairo Erkundung" },
                    { "it", "Piramidi & Esplorazione del Cairo" },
                    { "fr", "Pyramides & Exploration du Caire" },
                    { "ru", "Пирамиды и Каир" }
                }, 
                Descriptions = new Dictionary<string, string> { 
                    { "en", "Visit the Great Pyramids of Giza, the Sphinx, Egyptian Museum with Tutankhamun's treasures and Khan El Khalili bazaar." },
                    { "de", "Besuche die Großen Pyramiden von Gizeh, die Sphinx, das Ägyptische Museum mit Tutanchamuns Schätzen und den Khan El Khalili Basar." },
                    { "it", "Visita le Grandi Piramidi di Giza, la Sfinge, il Museo Egizio con i tesori di Tutankhamen e il bazar Khan El Khalili." },
                    { "fr", "Visitez les Grandes Pyramides de Gizeh, le Sphinx, le Musée Égyptien avec les trésors de Toutankhamon." },
                    { "ru", "Посетите Великие пирамиды Гизы, Сфинкс, Египетский музей с сокровищами Тутанхамона." }
                },
                Price = 180, 
                Duration = "twoDays",
                Includes = new List<string> { "✈️ Flights", "🏨 Hotel", "🎟️ Tickets", "🧭 Guide" },
                ImageUrl = "https://images.unsplash.com/photo-1503177119275-0aa32b3a9368",
                Emoji = "🏛️",
                BgGradient = "linear-gradient(135deg,#8b6914,#c9a84c)",
                Badge = "",
                DestinationId = destinations[2].Id,
                CategoryId = categories[1].Id
            },
            // 3. Luxor – Valley of Kings & Karnak
            new Tour { 
                Id = Guid.Parse("00000000-0000-0000-0000-000000000103"), 
                Names = new Dictionary<string, string> { 
                    { "en", "Luxor – Valley of Kings & Karnak" },
                    { "de", "Luxor – Tal der Könige & Karnak" },
                    { "it", "Luxor – Valle dei Re & Karnak" },
                    { "fr", "Louxor – Vallée des Rois & Karnak" },
                    { "ru", "Луксор – Долина царей и Карнак" }
                }, 
                Descriptions = new Dictionary<string, string> { 
                    { "en", "Explore the Valley of the Kings, Hatshepsut Temple, Karnak and Luxor Temples — Egypt's most magnificent ancient sites." },
                    { "de", "Erkunde das Tal der Könige, den Hatschepsut-Tempel, Karnak und Luxor-Tempel." },
                    { "it", "Esplora la Valle dei Re, il Tempio di Hatshepsut, Karnak e il Tempio di Luxor." },
                    { "fr", "Explorez la Vallée des Rois, le Temple d'Hatchepsout, Karnak et le Temple de Louxor." },
                    { "ru", "Долина царей, храм Хатшепсут, храмы Карнак и Луксор." }
                },
                Price = 160, 
                Duration = "twoDays",
                Includes = new List<string> { "✈️ Flights", "🏨 Hotel", "🧭 Guide" },
                ImageUrl = "https://images.unsplash.com/photo-1572252017416-224447c73410",
                Emoji = "🏺",
                BgGradient = "linear-gradient(135deg,#2e4a1a,#5a7c2a)",
                Badge = "",
                DestinationId = destinations[1].Id,
                CategoryId = categories[1].Id
            },
            // 4. Quad Bike & Bedouin Village
            new Tour { 
                Id = Guid.Parse("00000000-0000-0000-0000-000000000104"), 
                Names = new Dictionary<string, string> { 
                    { "en", "Quad Bike & Bedouin Village" },
                    { "de", "Quad Bike & Beduinendorf" },
                    { "it", "Quad Bike & Villaggio Beduino" },
                    { "fr", "Quad & Village Bédouin" },
                    { "ru", "Квадроциклы и деревня бедуинов" }
                }, 
                Descriptions = new Dictionary<string, string> { 
                    { "en", "Ride quad bikes through golden dunes, visit a traditional Bedouin village, enjoy camel riding and a stunning sunset." },
                    { "de", "Quad Bike durch goldene Dünen, Besuch eines traditionelles Beduinendorfes, Kamelreiten und atemberaubender Sonnenuntergang." },
                    { "it", "Giro in quad tra le dune dorate, visita a un villaggio beduino tradizionale, giro in cammello e tramonto spettacolare." },
                    { "fr", "Quad à travers les dunes dorées, village bédouin traditionnel, balade en chameau et coucher de soleil." },
                    { "ru", "Квадроциклы по золотым дюнам, деревня бедуинов, верблюды и закат." }
                },
                Price = 30, 
                Duration = "halfDay",
                Includes = new List<string> { "🚌 Transfer", "🍵 Bedouin Tea", "🐪 Camel Ride" },
                ImageUrl = "https://images.unsplash.com/photo-1539712232497-5813dec4431d",
                Emoji = "🏜️",
                BgGradient = "linear-gradient(135deg,#7c4a14,#e8820a)",
                Badge = "🔥 HOT",
                DestinationId = destinations[0].Id,
                CategoryId = categories[2].Id
            },
            // 5. Luxury Nile Cruise – Luxor to Aswan
            new Tour { 
                Id = Guid.Parse("00000000-0000-0000-0000-000000000105"), 
                Names = new Dictionary<string, string> { 
                    { "en", "Luxury Nile Cruise – Luxor to Aswan" },
                    { "de", "Luxus Nilkreuzfahrt – Luxor nach Assuan" },
                    { "it", "Crociera di lusso sul Nilo – Luxor ad Assuan" },
                    { "fr", "Croisière Nil de luxe – Louxor à Assouan" },
                    { "ru", "Круиз по Нилу – Луксор–Асуан" }
                }, 
                Descriptions = new Dictionary<string, string> { 
                    { "en", "Sail the legendary Nile on a 5-star cruise ship, visiting Edfu, Kom Ombo, Aswan Dam and the magnificent Abu Simbel temples." },
                    { "de", "Segeln Sie auf dem legendären Nil auf einem 5-Sterne-Kreuzfahrtschiff, Edfu, Kom Ombo, Assuan-Staudamm und die prächtigen Tempel von Abu Simbel." },
                    { "it", "Naviga sul leggendario Nilo su una nave da crociera 5 stelle, visitando Edfu, Kom Ombo, la diga di Assuan e i magnifici templi di Abu Simbel." },
                    { "fr", "Naviguez sur le légendaire Nil sur un bateau de croisière 5 étoiles, Edfou, Kom Ombo, barrage d'Assouan et les temples d'Abou Simbel." },
                    { "ru", "5-звёздочный круиз по Нилу: Эдфу, Ком Омбо, Асуанская плотина и храмы Абу-Симбел." }
                },
                Price = 490, 
                Duration = "fiveDays",
                Includes = new List<string> { "🛳️ Cruise", "🍽️ Full Board", "🎟️ All Tickets", "🧭 Guide" },
                ImageUrl = "https://images.unsplash.com/photo-1544551763-46a013bb70d5",
                Emoji = "🛳️",
                BgGradient = "linear-gradient(135deg,#063a5c,#c9a84c)",
                Badge = "⭐ LUXURY",
                DestinationId = destinations[1].Id,
                CategoryId = categories[1].Id
            },
            // 6. Scuba Diving – Ras Mohamed
            new Tour { 
                Id = Guid.Parse("00000000-0000-0000-0000-000000000106"), 
                Names = new Dictionary<string, string> { 
                    { "en", "Scuba Diving – Ras Mohamed" },
                    { "de", "Tauchen – Ras Mohamed" },
                    { "it", "Immersioni – Ras Mohamed" },
                    { "fr", "Plongée – Ras Mohamed" },
                    { "ru", "Дайвинг – Рас Мухаммад" }
                }, 
                Descriptions = new Dictionary<string, string> { 
                    { "en", "Dive in one of the world's top 10 dive sites with PADI certified instructors. Beginners and advanced divers welcome." },
                    { "de", "Tauchen Sie an einem der weltweit Top 10 Tauchplätze mit PADI-zertifizierten Instrukteuren. Anfänger und Fortgeschrittene willkommen." },
                    { "it", "Immersioni in uno dei 10 migliori siti di immersione al mondo con istruttori certificati PADI." },
                    { "fr", "Plongez dans l'un des 10 meilleurs sites de plongée au monde avec des instructeurs certifiés PADI." },
                    { "ru", "Дайвинг на одном из топ-10 мест для погружений в мире с инструкторами PADI." }
                },
                Price = 55, 
                Duration = "fullDay",
                Includes = new List<string> { "🚌 Transfer", "🤿 Equipment", "👨‍🏫 Instructor", "🥗 Lunch" },
                ImageUrl = "https://images.unsplash.com/photo-1544551763-46a013bb70d5",
                Emoji = "🐠",
                BgGradient = "linear-gradient(135deg,#0a3a5c,#1a9b70)",
                Badge = "",
                DestinationId = destinations[3].Id,
                CategoryId = categories[0].Id
            },
            // 7. Mount Sinai Sunrise & St Catherine
            new Tour { 
                Id = Guid.Parse("00000000-0000-0000-0000-000000000107"), 
                Names = new Dictionary<string, string> { 
                    { "en", "Mount Sinai Sunrise & St Catherine" },
                    { "de", "Sinai-Sonnenaufgang & St. Katharina" },
                    { "it", "Alba sul Sinai & Santa Caterina" },
                    { "fr", "Lever du soleil sur le Sinaï & Ste Catherine" },
                    { "ru", "Рассвет на Синае и монастырь Св. Екатерины" }
                }, 
                Descriptions = new Dictionary<string, string> { 
                    { "en", "Climb Mount Sinai by night, witness a breathtaking sunrise, visit the ancient monastery of St. Catherine — a spiritual journey." },
                    { "de", "Besteigen Sie den Berg Sinai bei Nacht, erleben Sie einen atemberaubenden Sonnenaufgang, besuchen Sie das antike Kloster der heiligen Katharina." },
                    { "it", "Scalate il Monte Sinai di notte, assistete a un'alba mozzafiato, visitate l'antico monastero di Santa Caterina." },
                    { "fr", "Gravissez le Mont Sinaï de nuit, admirez un lever de soleil époustouflant, visitez le monastère de Sainte-Catherine." },
                    { "ru", "Ночной подъём на гору Синай, рассвет, монастырь Св. Екатерины." }
                },
                Price = 45, 
                Duration = "oneDay",
                Includes = new List<string> { "🚌 Transfer", "🔦 Torch", "🧭 Guide" },
                ImageUrl = "https://images.unsplash.com/photo-1544551763-46a013bb70d5",
                Emoji = "⛪",
                BgGradient = "linear-gradient(135deg,#1a4060,#2e7d4f)",
                Badge = "",
                DestinationId = destinations[3].Id,
                CategoryId = categories[1].Id
            },
            // 8. Glass-Bottom Boat & Submarine
            new Tour { 
                Id = Guid.Parse("00000000-0000-0000-0000-000000000108"), 
                Names = new Dictionary<string, string> { 
                    { "en", "Glass-Bottom Boat & Submarine" },
                    { "de", "Glasbodenboot & U-Boot" },
                    { "it", "Barca con fondo di vetro & Sottomarino" },
                    { "fr", "Bateau à fond de verre & Sous-marin" },
                    { "ru", "Лодка со стеклянным дном и подводная лодка" }
                }, 
                Descriptions = new Dictionary<string, string> { 
                    { "en", "Experience the magical underwater world without getting wet — perfect for families. Observe exotic fish and corals in crystal clear water." },
                    { "de", "Erleben Sie die magische Unterwasserwelt, ohne nass zu werden — perfekt für Familien. Exotische Fische und Korallen beobachten." },
                    { "it", "Vivi il magico mondo sottomarino senza bagnarti — perfetto per le famiglie." },
                    { "fr", "Vivez le monde sous-marin sans vous mouiller — parfait pour les familles." },
                    { "ru", "Подводный мир без намокания — идеально для семей. Экзотические рыбы и кораллы." }
                },
                Price = 20, 
                Duration = "threeHours",
                Includes = new List<string> { "🚌 Transfer", "👨‍👩‍👧 Family OK" },
                ImageUrl = "https://images.unsplash.com/photo-1544551763-46a013bb70d5",
                Emoji = "🌊",
                BgGradient = "linear-gradient(135deg,#0d3050,#0a6e8a)",
                Badge = "",
                DestinationId = destinations[0].Id,
                CategoryId = categories[0].Id
            },
            // 9. Desert Safari & Stargazing Dinner
            new Tour { 
                Id = Guid.Parse("00000000-0000-0000-0000-000000000109"), 
                Names = new Dictionary<string, string> { 
                    { "en", "Desert Safari & Stargazing Dinner" },
                    { "de", "Wüsten-Safari & Sternenhimmel-Dinner" },
                    { "it", "Safari nel deserto & Cena sotto le stelle" },
                    { "fr", "Safari désert & Dîner sous les étoiles" },
                    { "ru", "Пустынное сафари и ужин под звёздами" }
                }, 
                Descriptions = new Dictionary<string, string> { 
                    { "en", "Sunset 4x4 desert safari, traditional Bedouin dinner under a billion stars, with live music and belly dance show." },
                    { "de", "Sonnenuntergang 4x4 Wüsten-Safari, traditionelles Beduinen-Dinner unter einem Millionen-Sternen-Himmel mit Live-Musik." },
                    { "it", "Safari nel deserto al tramonto in 4x4, cena beduina tradizionale sotto un cielo stellato, musica dal vivo." },
                    { "fr", "Safari 4x4 au coucher du soleil, dîner bédouin sous les étoiles, musique live et danse orientale." },
                    { "ru", "Сафари на 4x4 на закате, традиционный бедуинский ужин под звёздами, живая музыка и танец живота." }
                },
                Price = 35, 
                Duration = "evening",
                Includes = new List<string> { "🚌 Transfer", "🍽️ Dinner", "🎵 Show" },
                ImageUrl = "https://images.unsplash.com/photo-1539712232497-5813dec4431d",
                Emoji = "🌅",
                BgGradient = "linear-gradient(135deg,#5c3a0a,#a05c14)",
                Badge = "",
                DestinationId = destinations[0].Id,
                CategoryId = categories[2].Id
            }
        };

        context.Tours.AddRange(tours);
        await context.SaveChangesAsync();
    }
}
