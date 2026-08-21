using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Seadora.Booking.Domain.Entities;

namespace Seadora.Booking.Infrastructure.Persistence;

public static class BookingDbContextSeed
{
    public static async Task SeedAsync(BookingDbContext context)
    {
        // ponytail: no more EnsureDeleted — bookings and contact requests must survive redeploys.
        // Schema changes need EF migrations (none yet); add when the model changes.
        await context.Database.EnsureCreatedAsync();

        if (await context.Feedbacks.AnyAsync()) return;

        var feedbacks = new List<Feedback>
        {
            // Tour 1: Snorkeling Safari – Orange Bay
            new Feedback
            {
                Id = Guid.NewGuid(),
                TourId = Guid.Parse("00000000-0000-0000-0000-000000000101"),
                CustomerName = "Charlotte Sterling",
                CustomerEmail = "charlotte.sterling@example.com",
                Rating = 5,
                Comment = "Sailing on the Nile under the stars was pure magic. The private guide was incredibly knowledgeable and the dining was spectacular.",
                CreatedAt = DateTime.Parse("2026-05-18").ToUniversalTime()
            },
            new Feedback
            {
                Id = Guid.NewGuid(),
                TourId = Guid.Parse("00000000-0000-0000-0000-000000000101"),
                CustomerName = "Dr. Arthur Pendelton",
                CustomerEmail = "arthur.pendelton@example.com",
                Rating = 5,
                Comment = "An exceptional journey through Egypt's heritage. Every detail was curated with 5-star service. Highly recommend the sunset deck lounge.",
                CreatedAt = DateTime.Parse("2026-06-02").ToUniversalTime()
            },
            new Feedback
            {
                Id = Guid.NewGuid(),
                TourId = Guid.Parse("00000000-0000-0000-0000-000000000101"),
                CustomerName = "Sophia Vianni",
                CustomerEmail = "sophia.vianni@example.com",
                Rating = 4,
                Comment = "Fabulous views and comfortable cabins. Luxor temples are absolutely breathtaking at night. Minor delay at embarkation but resolved smoothly.",
                CreatedAt = DateTime.Parse("2026-06-10").ToUniversalTime()
            },

            // Tour 2: Pyramids & Cairo Explorer
            new Feedback
            {
                Id = Guid.NewGuid(),
                TourId = Guid.Parse("00000000-0000-0000-0000-000000000102"),
                CustomerName = "Maximilian Schwarz",
                CustomerEmail = "maximilian.schwarz@example.com",
                Rating = 5,
                Comment = "Crystal clear visibility and spectacular marine life. We swam alongside sea turtles and explored untouched corals. Unforgettable!",
                CreatedAt = DateTime.Parse("2026-04-20").ToUniversalTime()
            },
            new Feedback
            {
                Id = Guid.NewGuid(),
                TourId = Guid.Parse("00000000-0000-0000-0000-000000000102"),
                CustomerName = "Jessica Vance",
                CustomerEmail = "jessica.vance@example.com",
                Rating = 5,
                Comment = "The dive masters are true professionals. Safety and luxury service combined seamlessly. The yacht used for the dive was elite.",
                CreatedAt = DateTime.Parse("2026-05-14").ToUniversalTime()
            },

            // Tour 3: Luxor – Valley of Kings & Karnak
            new Feedback
            {
                Id = Guid.NewGuid(),
                TourId = Guid.Parse("00000000-0000-0000-0000-000000000103"),
                CustomerName = "Amina Al-Mansoor",
                CustomerEmail = "amina.almansoor@example.com",
                Rating = 5,
                Comment = "A captivating trek across the dunes. The Bedouin tea by the campfire under the Milky Way was a highlight of my year.",
                CreatedAt = DateTime.Parse("2026-03-30").ToUniversalTime()
            },
            new Feedback
            {
                Id = Guid.NewGuid(),
                TourId = Guid.Parse("00000000-0000-0000-0000-000000000103"),
                CustomerName = "Liam O'Connor",
                CustomerEmail = "liam.oconnor@example.com",
                Rating = 4,
                Comment = "Stunning landscapes and premium quad bikes. Very thrilling but also felt very safe and comfortable. The sunset photos are unreal.",
                CreatedAt = DateTime.Parse("2026-05-08").ToUniversalTime()
            }
        };

        // Seed fallback default comments for tours 4 to 9
        for (int i = 4; i <= 9; i++)
        {
            var tourId = Guid.Parse($"00000000-0000-0000-0000-00000000010{i}");
            feedbacks.Add(new Feedback
            {
                Id = Guid.NewGuid(),
                TourId = tourId,
                CustomerName = "Valerie Laurent",
                CustomerEmail = "valerie.laurent@example.com",
                Rating = 5,
                Comment = "Absolutely breathtaking! Seadora Travel provided a flawless, ultra-premium experience from start to finish.",
                CreatedAt = DateTime.Parse("2026-06-12").ToUniversalTime()
            });
            feedbacks.Add(new Feedback
            {
                Id = Guid.NewGuid(),
                TourId = tourId,
                CustomerName = "James Sinclair",
                CustomerEmail = "james.sinclair@example.com",
                Rating = 4,
                Comment = "Stunning scenery, professional staff, and superb coordination. True luxury in the heart of Egypt.",
                CreatedAt = DateTime.Parse("2026-06-15").ToUniversalTime()
            });
        }

        context.Feedbacks.AddRange(feedbacks);
        await context.SaveChangesAsync();
    }
}
